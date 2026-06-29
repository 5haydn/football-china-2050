import SwiftUI

/// Global keyboard shortcuts for match screen.
struct KeyboardControls: ViewModifier {
  let teams: [Team]
  @Binding var homeId: String
  @Binding var awayId: String
  let playing: Bool
  let canPlay: Bool
  let onPlay: () -> Void

  func body(content: Content) -> some View {
    content
      .onKeyPress(.space) { handlePlay(); return .handled }
      .onKeyPress(.return) { handlePlay(); return .handled }
      .onKeyPress(.leftArrow) { cycleHome(-1); return .handled }
      .onKeyPress(.rightArrow) { cycleHome(1); return .handled }
      .onKeyPress(.upArrow) { cycleAway(-1); return .handled }
      .onKeyPress(.downArrow) { cycleAway(1); return .handled }
      .onKeyPress(characters: CharacterSet(charactersIn: "hH")) { _ in cycleHome(-1); return .handled }
      .onKeyPress(characters: CharacterSet(charactersIn: "lL")) { _ in cycleHome(1); return .handled }
      .onKeyPress(characters: CharacterSet(charactersIn: "jJ")) { _ in cycleAway(-1); return .handled }
      .onKeyPress(characters: CharacterSet(charactersIn: "kK")) { _ in cycleAway(1); return .handled }
      .onKeyPress(characters: CharacterSet(charactersIn: "rR")) { _ in handlePlay(); return .handled }
  }

  private func handlePlay() {
    guard canPlay, !playing else { return }
    onPlay()
  }

  private func cycleHome(_ delta: Int) {
    guard !teams.isEmpty, !playing else { return }
    homeId = cycleId(homeId, delta: delta)
  }

  private func cycleAway(_ delta: Int) {
    guard !teams.isEmpty, !playing else { return }
    awayId = cycleId(awayId, delta: delta)
  }

  private func cycleId(_ current: String, delta: Int) -> String {
    guard let idx = teams.firstIndex(where: { $0.id == current }) else {
      return teams.first?.id ?? current
    }
    let next = (idx + delta + teams.count) % teams.count
    return teams[next].id
  }
}

extension View {
  func keyboardControls(
    teams: [Team],
    homeId: Binding<String>,
    awayId: Binding<String>,
    playing: Bool,
    canPlay: Bool,
    onPlay: @escaping () -> Void
  ) -> some View {
    modifier(KeyboardControls(
      teams: teams,
      homeId: homeId,
      awayId: awayId,
      playing: playing,
      canPlay: canPlay,
      onPlay: onPlay
    ))
  }
}

struct KeyboardHintsBar: View {
  var body: some View {
    HStack(spacing: 14) {
      hint("Space", "Kick off")
      hint("← →", "Home team")
      hint("↑ ↓", "Away team")
      hint("H/L J/K", "Teams")
    }
    .font(.system(size: 10, weight: .medium, design: .rounded))
    .foregroundStyle(GameTheme.muted)
    .frame(maxWidth: .infinity)
    .padding(.vertical, 8)
    .background(GameTheme.panel.opacity(0.6))
    .clipShape(RoundedRectangle(cornerRadius: 8, style: .continuous))
  }

  private func hint(_ key: String, _ action: String) -> some View {
    HStack(spacing: 4) {
      Text(key)
        .font(.system(size: 9, weight: .bold, design: .monospaced))
        .padding(.horizontal, 5)
        .padding(.vertical, 2)
        .background(GameTheme.panel)
        .clipShape(RoundedRectangle(cornerRadius: 4))
      Text(action)
    }
  }
}
