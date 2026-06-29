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
  @State private var ballX: CGFloat = 0
  @State private var ballY: CGFloat = 0
  @State private var ballHeight: CGFloat = 0

  var body: some View {
    ScrollView {
      VStack(spacing: 16) {
        AppHeader()

        if let error {
          Text(error)
            .font(.caption)
            .foregroundStyle(GameTheme.danger)
            .frame(maxWidth: .infinity, alignment: .leading)
        }

        HStack(spacing: 12) {
          TeamPickerCard(title: "Home", accent: GameTheme.home, selection: $homeId, teams: teams)
          TeamPickerCard(title: "Away", accent: GameTheme.away, selection: $awayId, teams: teams)
        }

        ScoreboardView(
          homeName: teams.first { $0.id == homeId }?.name ?? "—",
          awayName: teams.first { $0.id == awayId }?.name ?? "—",
          homeScore: homeScore,
          awayScore: awayScore
        )

        PitchFrame(ballX: ballX, ballY: ballY, ballHeight: ballHeight)

        EventLogView(events: events)

        Button(action: playMatch) {
          HStack(spacing: 8) {
            Image(systemName: playing ? "hourglass" : "play.fill")
            Text(playing ? "Match in progress…" : events.isEmpty ? "Kick Off" : "Play Again")
            if !playing && canPlay {
              Text("␣")
                .font(.system(size: 11, weight: .bold, design: .monospaced))
                .foregroundStyle(Color.black.opacity(0.45))
            }
          }
        }
        .buttonStyle(PrimaryButtonStyle(disabled: !canPlay))
        .disabled(!canPlay)

        KeyboardHintsBar()
      }
      .padding(20)
    }
    .frame(minWidth: 560, minHeight: 760)
    .background(GameTheme.background)
    .focusable()
    .keyboardControls(
      teams: teams,
      homeId: $homeId,
      awayId: $awayId,
      playing: playing,
      canPlay: canPlay,
      onPlay: playMatch
    )
    .task { loadTeams() }
  }

  private var canPlay: Bool {
    !teams.isEmpty && !playing && homeId != awayId
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
    ballX = 0
    ballY = 0
    ballHeight = 0

    let result = MatchSimulator.simulate(home: home, away: away, seed: UInt64(Date().timeIntervalSince1970))

    Task {
      for event in result.events {
        try? await Task.sleep(for: .milliseconds(450))
        withAnimation(.easeOut(duration: 0.2)) {
          events.append(event)
        }
        await animateBall(toX: event.pitchX, toY: event.pitchY, isGoal: event.isGoal)
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

  private func animateBall(toX: CGFloat, toY: CGFloat, isGoal: Bool) async {
    let peak: CGFloat = isGoal ? 1.2 : 0.5
    let steps = 8
    let startX = ballX
    let startY = ballY
    for i in 1...steps {
      let t = CGFloat(i) / CGFloat(steps)
      ballX = startX + (toX - startX) * t
      ballY = startY + (toY - startY) * t
      ballHeight = sin(t * .pi) * peak
      try? await Task.sleep(for: .milliseconds(25))
    }
    ballHeight = 0
  }
}
