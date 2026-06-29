import SceneKit
import SwiftUI

/// Full 3D pitch rendered with SceneKit.
struct Pitch3DSceneView: NSViewRepresentable {
  let homeColor: NSColor
  let awayColor: NSColor
  var ballX: CGFloat = 0
  var ballY: CGFloat = 0
  var ballHeight: CGFloat = 0

  func makeNSView(context: Context) -> SCNView {
    let view = SCNView()
    view.scene = context.coordinator.scene
    view.allowsCameraControl = false
    view.autoenablesDefaultLighting = true
    view.backgroundColor = NSColor(red: 0.05, green: 0.08, blue: 0.12, alpha: 1)
    view.antialiasingMode = .multisampling4X
    return view
  }

  func updateNSView(_ view: SCNView, context: Context) {
    context.coordinator.updateBall(x: ballX, y: ballY, height: ballHeight)
  }

  func makeCoordinator() -> Coordinator {
    Coordinator(homeColor: homeColor, awayColor: awayColor)
  }

  final class Coordinator {
    let scene = SCNScene()
    let ballNode: SCNNode

    init(homeColor: NSColor, awayColor: NSColor) {
      ballNode = SCNNode(geometry: SCNSphere(radius: 0.22))
      ballNode.geometry?.firstMaterial?.diffuse.contents = NSColor.white

      buildPitch()
      buildGoals()
      buildPlayers(home: homeColor, away: awayColor)

      scene.rootNode.addChildNode(ballNode)
      ballNode.position = SCNVector3(0, 0.35, 0)

      let camera = SCNNode()
      camera.camera = SCNCamera()
      camera.camera?.fieldOfView = 48
      camera.position = SCNVector3(0, 22, -18)
      camera.look(at: SCNVector3(0, 0, 4))
      scene.rootNode.addChildNode(camera)

      let light = SCNNode()
      light.light = SCNLight()
      light.light?.type = .directional
      light.eulerAngles = SCNVector3(-Float.pi / 3, Float.pi / 4, 0)
      scene.rootNode.addChildNode(light)

      let ambient = SCNNode()
      ambient.light = SCNLight()
      ambient.light?.type = .ambient
      ambient.light?.color = NSColor(white: 0.55, alpha: 1)
      scene.rootNode.addChildNode(ambient)
    }

    func updateBall(x: CGFloat, y: CGFloat, height: CGFloat) {
      let fieldW: Float = 20
      let fieldL: Float = 32
      ballNode.position = SCNVector3(Float(x) * fieldW, 0.35 + Float(height) * 2.5, Float(y) * fieldL)
    }

    private func buildPitch() {
      let grass = SCNPlane(width: 20, height: 32)
      grass.firstMaterial?.diffuse.contents = NSColor(red: 0.2, green: 0.55, blue: 0.24, alpha: 1)
      let grassNode = SCNNode(geometry: grass)
      grassNode.eulerAngles.x = -.pi / 2
      scene.rootNode.addChildNode(grassNode)

      let base = SCNBox(width: 28, height: 1, length: 40, chamferRadius: 0)
      base.firstMaterial?.diffuse.contents = NSColor(red: 0.08, green: 0.12, blue: 0.16, alpha: 1)
      let baseNode = SCNNode(geometry: base)
      baseNode.position = SCNVector3(0, -0.55, 0)
      scene.rootNode.addChildNode(baseNode)
    }

    private func buildGoals() {
      for z: Float in [-15.2, 15.2] {
        let goal = SCNNode()
        goal.position = SCNVector3(0, 0, z)

        let postL = box(w: 0.12, h: 1.3, l: 0.12, color: .white)
        postL.position = SCNVector3(-1.3, 0.65, 0)
        goal.addChildNode(postL)

        let postR = box(w: 0.12, h: 1.3, l: 0.12, color: .white)
        postR.position = SCNVector3(1.3, 0.65, 0)
        goal.addChildNode(postR)

        let cross = box(w: 2.7, h: 0.12, l: 0.12, color: .white)
        cross.position = SCNVector3(0, 1.25, 0)
        goal.addChildNode(cross)

        scene.rootNode.addChildNode(goal)
      }
    }

    private func buildPlayers(home: NSColor, away: NSColor) {
      let homePos: [(Float, Float)] = [(0, -11), (-5.5, -6), (5.5, -6), (-2.5, -13), (2.5, -13)]
      let awayPos: [(Float, Float)] = [(0, 11), (5.5, 6), (-5.5, 6), (2.5, 13), (-2.5, 13)]

      for (i, p) in homePos.enumerated() {
        let node = capsule(color: home, label: "\(i + 1)")
        node.position = SCNVector3(p.0, 0.7, p.1)
        scene.rootNode.addChildNode(node)
      }
      for (i, p) in awayPos.enumerated() {
        let node = capsule(color: away, label: "\(i + 1)")
        node.position = SCNVector3(p.0, 0.7, p.1)
        scene.rootNode.addChildNode(node)
      }
    }

    private func capsule(color: NSColor, label: String) -> SCNNode {
      let cap = SCNCapsule(capRadius: 0.28, height: 1.2)
      cap.firstMaterial?.diffuse.contents = color
      let node = SCNNode(geometry: cap)
      let text = SCNText(string: label, extrusionDepth: 0.05)
      text.font = NSFont.boldSystemFont(ofSize: 3)
      text.firstMaterial?.diffuse.contents = NSColor.white
      let textNode = SCNNode(geometry: text)
      textNode.scale = SCNVector3(0.08, 0.08, 0.08)
      textNode.position = SCNVector3(-0.08, 0.5, 0)
      node.addChildNode(textNode)
      return node
    }

    private func box(w: CGFloat, h: CGFloat, l: CGFloat, color: NSColor) -> SCNNode {
      let geo = SCNBox(width: w, height: h, length: l, chamferRadius: 0)
      geo.firstMaterial?.diffuse.contents = color
      return SCNNode(geometry: geo)
    }
  }
}
