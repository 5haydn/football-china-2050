import Foundation

struct MatchResult {
  let events: [MatchEvent]
  let homeScore: Int
  let awayScore: Int
}

enum MatchSimulator {
  private static let phases = 24
  private static let squad = 5

  static func simulate(home: Team, away: Team, seed: UInt64) -> MatchResult {
    var rng = seed
    var homeScore = 0
    var awayScore = 0
    var events: [MatchEvent] = []

    func rand() -> Double {
      rng = rng &* 1103515245 &+ 12345
      return Double(rng & 0x7fffffff) / Double(0x7fffffff)
    }

    events.append(MatchEvent(minute: 0, text: "\(home.name) vs \(away.name) — kickoff!", isGoal: false, isHalf: false))

    for phase in 0..<phases {
      let minute = min(90, Int(Double(phase + 1) / Double(phases) * 90))
      if phase == phases / 2 {
        events.append(MatchEvent(minute: minute, text: "Half time.", isGoal: false, isHalf: true))
      }

      let attack = rand() > 0.5 ? home : away
      let defend = attack.id == home.id ? away : home
      let atk = power(attack, sw: 0.45, pw: 0.35, dw: 0.2)
      let def = power(defend, sw: 0.15, pw: 0.25, dw: 0.6)
      let roll = rand() * (atk + def)
      let shooter = pick(attack, shoot: true)

      if roll < atk * 0.12 {
        if attack.id == home.id { homeScore += 1 } else { awayScore += 1 }
        events.append(MatchEvent(
          minute: minute,
          text: "GOAL! \(shooter.name) (\(attack.name)) \(homeScore)-\(awayScore)",
          isGoal: true,
          isHalf: false
        ))
      } else if roll < atk * 0.35 {
        events.append(MatchEvent(minute: minute, text: "\(shooter.name) shoots — saved!", isGoal: false, isHalf: false))
      } else if roll < atk * 0.55 {
        events.append(MatchEvent(minute: minute, text: "\(defend.name) win the ball.", isGoal: false, isHalf: false))
      } else {
        events.append(MatchEvent(minute: minute, text: "\(shooter.name) — shot wide.", isGoal: false, isHalf: false))
      }
    }

    events.append(MatchEvent(
      minute: 90,
      text: "Full time: \(home.name) \(homeScore) - \(awayScore) \(away.name)",
      isGoal: true,
      isHalf: false
    ))

    return MatchResult(events: events, homeScore: homeScore, awayScore: awayScore)
  }

  private static func power(_ team: Team, sw: Double, pw: Double, dw: Double) -> Double {
    let squad = Array(team.players.prefix(squad))
    guard !squad.isEmpty else { return 50 }
    return squad.reduce(0.0) { $0 + Double($1.shoot) * sw + Double($1.pass) * pw + Double($1.defend) * dw + Double($1.pace) * 0.1 } / Double(squad.count)
  }

  private static func pick(_ team: Team, shoot: Bool) -> Player {
    let squad = Array(team.players.prefix(squad))
    return squad.sorted { shoot ? $0.shoot > $1.shoot : $0.pass > $1.pass }.first!
  }
}
