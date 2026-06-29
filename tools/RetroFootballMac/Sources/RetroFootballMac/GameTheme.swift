import SwiftUI

enum GameTheme {
  static let bgTop = Color(red: 0.04, green: 0.07, blue: 0.11)
  static let bgBottom = Color(red: 0.08, green: 0.13, blue: 0.18)
  static let panel = Color(red: 0.11, green: 0.16, blue: 0.22)
  static let panelBorder = Color.white.opacity(0.08)
  static let accent = Color(red: 0.0, green: 0.82, blue: 0.65)
  static let gold = Color(red: 0.96, green: 0.72, blue: 0.28)
  static let cream = Color(red: 0.94, green: 0.91, blue: 0.84)
  static let muted = Color(red: 0.55, green: 0.62, blue: 0.7)
  static let home = Color(red: 0.92, green: 0.22, blue: 0.18)
  static let away = Color(red: 0.18, green: 0.52, blue: 0.95)
  static let danger = Color(red: 0.95, green: 0.35, blue: 0.35)

  static var background: LinearGradient {
    LinearGradient(colors: [bgTop, bgBottom], startPoint: .top, endPoint: .bottom)
  }

  static var accentGradient: LinearGradient {
    LinearGradient(
      colors: [Color(red: 0.0, green: 0.9, blue: 0.7), Color(red: 0.0, green: 0.65, blue: 0.55)],
      startPoint: .topLeading,
      endPoint: .bottomTrailing
    )
  }
}

struct PanelStyle: ViewModifier {
  var padding: CGFloat = 16

  func body(content: Content) -> some View {
    content
      .padding(padding)
      .background(GameTheme.panel)
      .overlay(
        RoundedRectangle(cornerRadius: 14, style: .continuous)
          .stroke(GameTheme.panelBorder, lineWidth: 1)
      )
      .clipShape(RoundedRectangle(cornerRadius: 14, style: .continuous))
  }
}

extension View {
  func panelStyle(padding: CGFloat = 16) -> some View {
    modifier(PanelStyle(padding: padding))
  }
}

struct PrimaryButtonStyle: ButtonStyle {
  var disabled: Bool = false

  func makeBody(configuration: Configuration) -> some View {
    configuration.label
      .font(.system(size: 15, weight: .bold, design: .rounded))
      .foregroundStyle(disabled ? GameTheme.muted : Color.black.opacity(0.85))
      .frame(maxWidth: .infinity)
      .padding(.vertical, 14)
      .background(
        Group {
          if disabled {
            GameTheme.panel
          } else {
            GameTheme.accentGradient
          }
        }
      )
      .clipShape(RoundedRectangle(cornerRadius: 12, style: .continuous))
      .overlay(
        RoundedRectangle(cornerRadius: 12, style: .continuous)
          .stroke(disabled ? GameTheme.panelBorder : Color.white.opacity(0.25), lineWidth: 1)
      )
      .scaleEffect(configuration.isPressed && !disabled ? 0.98 : 1)
      .animation(.easeOut(duration: 0.12), value: configuration.isPressed)
  }
}
