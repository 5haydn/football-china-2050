import Foundation

enum TeamsLoader {
  static func load() throws -> [Team] {
    let candidates = [
      ProcessInfo.processInfo.environment["FOOTBALL_ROOT"].map {
        URL(fileURLWithPath: $0)
          .appendingPathComponent("Assets/StreamingAssets/Historical/teams.json")
      },
      URL(fileURLWithPath: "/Users/haydn/Desktop/football-china-2050/Assets/StreamingAssets/Historical/teams.json"),
      Bundle.main.bundleURL
        .deletingLastPathComponent()
        .deletingLastPathComponent()
        .deletingLastPathComponent()
        .deletingLastPathComponent()
        .appendingPathComponent("Assets/StreamingAssets/Historical/teams.json")
    ].compactMap { $0 }

    for url in candidates where FileManager.default.fileExists(atPath: url.path) {
      let data = try Data(contentsOf: url)
      return try JSONDecoder().decode(TeamsFile.self, from: data).teams
    }
    throw NSError(domain: "RetroFootball", code: 1, userInfo: [
      NSLocalizedDescriptionKey: "teams.json not found"
    ])
  }
}
