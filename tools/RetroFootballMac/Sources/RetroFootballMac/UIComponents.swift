import SwiftUI

struct AppHeader: View {
  var body: some View {
    VStack(spacing: 6) {
      HStack(spacing: 10) {
        Text("⚽")
          .font(.system(size: 28))
        VStack(alignment: .leading, spacing: 2) {
          Text("RETRO FOOTBALL")
            .font(.system(size: 11, weight: .heavy, design: .rounded))
            .tracking(3)
            .foregroundStyle(GameTheme.gold)
          Text("'76")
            .font(.system(size: 32, weight: .black, design: .rounded))
            .foregroundStyle(GameTheme.cream)
        }
        Spacer()
        VStack(alignment: .trailing, spacing: 2) {
          Text("LIVE")
            .font(.system(size: 9, weight: .bold, design: .rounded))
            .tracking(2)
            .foregroundStyle(GameTheme.accent)
          Text("1974 · 1976")
            .font(.system(size: 11, weight: .medium, design: .rounded))
            .foregroundStyle(GameTheme.muted)
        }
        .padding(.horizontal, 10)
        .padding(.vertical, 6)
        .background(GameTheme.panel)
        .clipShape(RoundedRectangle(cornerRadius: 8, style: .continuous))
      }
      Rectangle()
        .fill(
          LinearGradient(
            colors: [GameTheme.gold.opacity(0.8), GameTheme.accent.opacity(0.6), .clear],
            startPoint: .leading,
            endPoint: .trailing
          )
        )
        .frame(height: 2)
    }
  }
}

struct TeamPickerCard: View {
  let title: String
  let accent: Color
  @Binding var selection: String
  let teams: [Team]

  var body: some View {
    VStack(alignment: .leading, spacing: 8) {
      HStack(spacing: 6) {
        Circle().fill(accent).frame(width: 8, height: 8)
        Text(title.uppercased())
          .font(.system(size: 10, weight: .bold, design: .rounded))
          .tracking(1.5)
          .foregroundStyle(GameTheme.muted)
      }

      Picker(title, selection: $selection) {
        ForEach(teams) { t in
          Text("\(t.name) (\(t.year))").tag(t.id)
        }
      }
      .labelsHidden()
      .tint(accent)
    }
    .frame(maxWidth: .infinity, alignment: .leading)
    .panelStyle(padding: 12)
  }
}

struct ScoreboardView: View {
  let homeName: String
  let awayName: String
  let homeScore: Int
  let awayScore: Int

  var body: some View {
    HStack(spacing: 0) {
      teamColumn(name: homeName, score: homeScore, color: GameTheme.home, align: .leading)

      VStack(spacing: 4) {
        Text("FULL TIME")
          .font(.system(size: 8, weight: .bold, design: .rounded))
          .tracking(2)
          .foregroundStyle(GameTheme.muted)
        Text("—")
          .font(.system(size: 22, weight: .light))
          .foregroundStyle(GameTheme.gold.opacity(0.6))
      }
      .frame(width: 56)

      teamColumn(name: awayName, score: awayScore, color: GameTheme.away, align: .trailing)
    }
    .padding(.vertical, 14)
    .padding(.horizontal, 16)
    .background(
      ZStack {
        GameTheme.panel
        LinearGradient(
          colors: [GameTheme.gold.opacity(0.06), .clear],
          startPoint: .top,
          endPoint: .bottom
        )
      }
    )
    .overlay(
      RoundedRectangle(cornerRadius: 14, style: .continuous)
        .stroke(
          LinearGradient(
            colors: [GameTheme.gold.opacity(0.35), GameTheme.panelBorder],
            startPoint: .topLeading,
            endPoint: .bottomTrailing
          ),
          lineWidth: 1
        )
    )
    .clipShape(RoundedRectangle(cornerRadius: 14, style: .continuous))
  }

  private func teamColumn(name: String, score: Int, color: Color, align: HorizontalAlignment) -> some View {
    VStack(alignment: align, spacing: 6) {
      HStack(spacing: 6) {
        if align == .trailing { Spacer(minLength: 0) }
        Circle().fill(color).frame(width: 6, height: 6)
        Text(name)
          .font(.system(size: 13, weight: .semibold, design: .rounded))
          .foregroundStyle(GameTheme.cream)
          .lineLimit(1)
          .minimumScaleFactor(0.7)
        if align == .leading { Spacer(minLength: 0) }
      }
      Text("\(score)")
        .font(.system(size: 36, weight: .black, design: .rounded))
        .foregroundStyle(color)
        .monospacedDigit()
    }
    .frame(maxWidth: .infinity)
  }
}

struct MatchEventRow: View {
  let event: MatchEvent

  var body: some View {
    HStack(alignment: .top, spacing: 10) {
      Text(String(format: "%02d'", event.minute))
        .font(.system(size: 11, weight: .bold, design: .monospaced))
        .foregroundStyle(GameTheme.muted)
        .frame(width: 32, alignment: .leading)

      Rectangle()
        .fill(accentColor)
        .frame(width: 3)
        .clipShape(Capsule())

      Text(event.text)
        .font(.system(size: 13, weight: event.isGoal ? .bold : .regular, design: .rounded))
        .foregroundStyle(textColor)
        .frame(maxWidth: .infinity, alignment: .leading)
    }
    .padding(.vertical, 4)
  }

  private var accentColor: Color {
    if event.isGoal { return GameTheme.accent }
    if event.isHalf { return GameTheme.gold }
    return GameTheme.panelBorder
  }

  private var textColor: Color {
    if event.isGoal { return GameTheme.accent }
    if event.isHalf { return GameTheme.gold }
    return GameTheme.cream.opacity(0.9)
  }
}

struct EventLogView: View {
  let events: [MatchEvent]

  var body: some View {
    VStack(alignment: .leading, spacing: 8) {
      Text("MATCH FEED")
        .font(.system(size: 10, weight: .bold, design: .rounded))
        .tracking(2)
        .foregroundStyle(GameTheme.muted)

      ScrollViewReader { proxy in
        ScrollView {
          LazyVStack(alignment: .leading, spacing: 2) {
            if events.isEmpty {
              Text("Press Play to kick off…")
                .font(.system(size: 13, design: .rounded))
                .foregroundStyle(GameTheme.muted)
                .padding(.top, 8)
            } else {
              ForEach(events) { e in
                MatchEventRow(event: e).id(e.id)
              }
            }
          }
        }
        .onChange(of: events.count) { _, _ in
          if let last = events.last {
            withAnimation { proxy.scrollTo(last.id, anchor: .bottom) }
          }
        }
      }
    }
    .panelStyle(padding: 12)
    .frame(height: 150)
  }
}

struct PitchFrame: View {
  let ballX: CGFloat
  let ballY: CGFloat
  let ballHeight: CGFloat

  var body: some View {
    VStack(spacing: 0) {
      HStack {
        Text("3D PITCH")
          .font(.system(size: 10, weight: .bold, design: .rounded))
          .tracking(2)
          .foregroundStyle(GameTheme.muted)
        Spacer()
        Text("BROADCAST CAM")
          .font(.system(size: 9, weight: .medium, design: .rounded))
          .foregroundStyle(GameTheme.accent.opacity(0.8))
      }
      .padding(.horizontal, 14)
      .padding(.top, 12)
      .padding(.bottom, 8)

      Pitch3DSceneView(
        homeColor: NSColor(red: 0.92, green: 0.22, blue: 0.18, alpha: 1),
        awayColor: NSColor(red: 0.18, green: 0.52, blue: 0.95, alpha: 1),
        ballX: ballX,
        ballY: ballY,
        ballHeight: ballHeight
      )
      .frame(height: 240)
      .clipShape(RoundedRectangle(cornerRadius: 10, style: .continuous))
      .padding(.horizontal, 10)
      .padding(.bottom, 12)
    }
    .background(GameTheme.panel)
    .overlay(
      RoundedRectangle(cornerRadius: 14, style: .continuous)
        .stroke(GameTheme.panelBorder, lineWidth: 1)
    )
    .clipShape(RoundedRectangle(cornerRadius: 14, style: .continuous))
  }
}
