import SwiftUI

/// Isometric-style 2.5D pitch with animated ball and player markers.
struct Pitch2_5DView: View {
  let homeColor: Color
  let awayColor: Color
  var ballX: CGFloat = 0
  var ballY: CGFloat = 0
  var ballHeight: CGFloat = 0

  var body: some View {
    GeometryReader { geo in
      let center = CGPoint(x: geo.size.width / 2, y: geo.size.height * 0.55)
      let scale = min(geo.size.width, geo.size.height) * 0.38

      ZStack {
        // Depth grass layer
        fieldPath(center: center, scale: scale * 1.08, skew: 0.22)
          .fill(Color(red: 0.1, green: 0.32, blue: 0.14))

        // Main pitch surface
        fieldPath(center: center, scale: scale, skew: 0.22)
          .fill(
            LinearGradient(
              colors: [
                Color(red: 0.18, green: 0.52, blue: 0.22),
                Color(red: 0.14, green: 0.44, blue: 0.18)
              ],
              startPoint: .topLeading,
              endPoint: .bottomTrailing
            )
          )

        fieldPath(center: center, scale: scale, skew: 0.22)
          .stroke(Color.white.opacity(0.45), lineWidth: 2)

        // Center line
        Path { p in
          let a = iso(center: center, x: -0.48, y: 0, scale: scale, skew: 0.22)
          let b = iso(center: center, x: 0.48, y: 0, scale: scale, skew: 0.22)
          p.move(to: a)
          p.addLine(to: b)
        }
        .stroke(Color.white.opacity(0.35), lineWidth: 1.5)

        // Home players (bottom / near)
        ForEach(0..<5, id: \.self) { i in
          playerDot(
            at: formationHome[i],
            center: center,
            scale: scale,
            color: homeColor,
            label: "\(i + 1)"
          )
        }

        // Away players (top / far)
        ForEach(0..<5, id: \.self) { i in
          playerDot(
            at: formationAway[i],
            center: center,
            scale: scale,
            color: awayColor,
            label: "\(i + 1)"
          )
        }

        // Ball shadow
        Circle()
          .fill(Color.black.opacity(0.35))
          .frame(width: 14, height: 8)
          .position(iso(center: center, x: ballX, y: ballY, scale: scale, skew: 0.22))

        // Ball with height arc (2.5D)
        Circle()
          .fill(Color.white)
          .overlay(Circle().stroke(Color.black.opacity(0.15), lineWidth: 1))
          .frame(width: 12, height: 12)
          .position(
            iso(center: center, x: ballX, y: ballY, scale: scale, skew: 0.22)
              .applying(.init(translationX: 0, y: -ballHeight * scale * 0.25))
          )
          .shadow(color: .black.opacity(0.3), radius: 2, y: 2)
      }
    }
  }

  private let formationHome: [(CGFloat, CGFloat)] = [
    (0, -0.35), (-0.25, -0.2), (0.25, -0.2), (-0.12, -0.42), (0.12, -0.42)
  ]

  private let formationAway: [(CGFloat, CGFloat)] = [
    (0, 0.35), (0.25, 0.2), (-0.25, 0.2), (0.12, 0.42), (-0.12, 0.42)
  ]

  private func fieldPath(center: CGPoint, scale: CGFloat, skew: CGFloat) -> Path {
    Path { p in
      let pts: [(CGFloat, CGFloat)] = [
        (-0.5, -0.5), (0.5, -0.5), (0.5, 0.5), (-0.5, 0.5)
      ]
      let isoPts = pts.map { iso(center: center, x: $0.0, y: $0.1, scale: scale, skew: skew) }
      p.move(to: isoPts[0])
      for pt in isoPts.dropFirst() { p.addLine(to: pt) }
      p.closeSubpath()
    }
  }

  private func playerDot(at pos: (CGFloat, CGFloat), center: CGPoint, scale: CGFloat, color: Color, label: String) -> some View {
    let pt = iso(center: center, x: pos.0, y: pos.1, scale: scale, skew: 0.22)
    return ZStack {
      Circle()
        .fill(Color.black.opacity(0.3))
        .frame(width: 18, height: 10)
        .position(x: pt.x, y: pt.y + 6)
      Circle()
        .fill(color)
        .frame(width: 16, height: 16)
        .overlay(
          Text(label)
            .font(.system(size: 8, weight: .bold))
            .foregroundStyle(.white)
        )
        .position(pt)
    }
  }

  private func iso(center: CGPoint, x: CGFloat, y: CGFloat, scale: CGFloat, skew: CGFloat) -> CGPoint {
    // 2.5D isometric projection
    CGPoint(
      x: center.x + (x - y * skew) * scale,
      y: center.y + (x * skew + y) * scale * 0.55
    )
  }
}
