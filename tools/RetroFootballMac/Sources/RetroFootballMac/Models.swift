import Foundation

struct TeamsFile: Decodable {
  let teams: [Team]
}

struct Team: Decodable, Identifiable, Hashable {
  let id: String
  let name: String
  let year: Int
  let formation: String?
  let players: [Player]
}

struct Player: Decodable, Hashable {
  let name: String
  let position: String
  let shoot: Int
  let pass: Int
  let defend: Int
  let pace: Int
}

struct MatchEvent: Identifiable {
  let id = UUID()
  let minute: Int
  let text: String
  let isGoal: Bool
  let isHalf: Bool
  let pitchX: CGFloat
  let pitchY: CGFloat
}
