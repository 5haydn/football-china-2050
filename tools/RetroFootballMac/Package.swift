// swift-tools-version: 5.9
import PackageDescription

let package = Package(
  name: "RetroFootballMac",
  platforms: [.macOS(.v14)],
  targets: [
    .executableTarget(name: "RetroFootballMac")
  ]
)
