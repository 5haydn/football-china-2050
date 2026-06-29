import SwiftUI

struct ContentView: View {
  @State private var teams: [Team] = []
  @State private var homeId = "ned1974"
  @State private var awayId = "frg1974"
  @State private var events: [MatchEvent] = []
  @State private var homeScore = 0
  @State private var awayScore = 0
  @State private var playing = false
  @State private var error: String?

  private let bg = Color(red: 0.06, green: 0.11, blue: 0.14)
  private let card = Color(red: 0.12, green: 0.18, blue: 0.24)
  private let accent = Color(red: 0.0, green: 0.83, blue: 0.67)
  private let gold = Color(red: 0.94, green: 0.63, blue: 0.25)

  var body: some View {
    VStack(spacing: 16) {
      Text("⚽ Retro Football '76")
        .font(.title2.bold())
      Text("1974 World Cup · 1976 Euro")
        .font(.caption)
        .foregroundStyle(.secondary)

      if let error {
        Text(error).foregroundStyle(.red).font(.caption)
      }

      HStack(spacing: 12) {
        teamPicker(title: "Home", selection: $homeId)
        teamPicker(title: "Away", selection: $awayId)
      }

      scoreboard

      ScrollViewReader { proxy in
        ScrollView {
          LazyVStack(alignment: .leading, spacing: 6) {
            ForEach(events) { e in
              HStack(alignment: .top, spacing: 8) {
                Text(String(format: "%2d'", e.minute))
                  .font(.caption.monospacedDigit())
                  .foregroundStyle(.secondary)
                  .frame(width: 28, alignment: .leading)
                Text(e.text)
                  .font(.system(size: 13))
                  .foregroundStyle(e.isGoal ? accent : e.isHalf ? gold : .primary)
              }
              .id(e.id)
            }
          }
          .padding(12)
        }
        .frame(height: 300)
        .background(Color.black.opacity(0.35))
        .clipShape(RoundedRectangle(cornerRadius: 8))
        .onChange(of: events.count) { _, _ in
          if let last = events.last {
            withAnimation { proxy.scrollTo(last.id, anchor: .bottom) }
          }
        }
      }

      Button(action: playMatch) {
        Text(playing ? "Playing…" : events.isEmpty ? "▶ Play Match" : "▶ Play Again")
          .font(.headline)
          .frame(maxWidth: .infinity)
          .padding(.vertical, 12)
      }
      .buttonStyle(.borderedProminent)
      .tint(accent)
      .disabled(playing || teams.isEmpty || homeId == awayId)
    }
    .padding(24)
    .frame(minWidth: 480, minHeight: 580)
    .background(bg)
    .task { loadTeams() }
  }

  private func teamPicker(title: String, selection: Binding<String>) -> some View {
    VStack(alignment: .leading, spacing: 4) {
      Text(title).font(.caption).foregroundStyle(.secondary)
      Picker(title, selection: selection) {
        ForEach(teams) { t in
          Text("\(t.name) (\(t.year)").tag(t.id)
        }
      }
      .labelsHidden()
    }
  }

  private var scoreboard: some View {
    let home = teams.first { $0.id == homeId }
    let away = teams.first { $0.id == awayId }
  return Text("\(home?.name ?? "—")  \(homeScore)  –  \(awayScore)  \(away?.name ?? "—")")
      .font(.title.bold())
      .multilineTextAlignment(.center)
      .padding()
      .frame(maxWidth: .infinity)
      .background(card)
      .clipShape(RoundedRectangle(cornerRadius: 8))
  }

  private func loadTeams() {
    do {
      teams = try TeamsLoader.load()
    } catch {
      self.error = error.localizedDescription
    }
  }

  private func playMatch() {
    guard let home = teams.first(where: { $0.id == homeId }),
          let away = teams.first(where: { $0.id == awayId }) else { return }

    playing = true
    events = []
    homeScore = 0
    awayScore = 0

    let result = MatchSimulator.simulate(home: home, away: away, seed: UInt64(Date().timeIntervalSince1970))

    Task {
      for event in result.events {
        try? await Task.sleep(for: .milliseconds(450))
        events.append(event)
        if event.text.hasPrefix("GOAL!") || event.minute == 90 {
          if let score = event.text.range(of: #"\d+-\d+"#, options: .regularExpression) {
            let parts = event.text[score].split(separator: "-")
            if parts.count == 2 {
              homeScore = Int(parts[0]) ?? homeScore
              awayScore = Int(parts[1]) ?? awayScore
            }
          }
        }
      }
      homeScore = result.homeScore
      awayScore = result.awayScore
      playing = false
    }
  }
}
