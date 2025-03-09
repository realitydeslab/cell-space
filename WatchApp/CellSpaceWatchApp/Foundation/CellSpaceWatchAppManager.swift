// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

import Foundation
import WatchKit
import WatchConnectivity
import HealthKit

// Make sure this enum is identical to the corresponding C# enum
enum CellSpaceWatchPanel: Int {
    case none = 0
    case biofeedback = 1
}

class CellSpaceWatchAppManager: NSObject, ObservableObject {
    
    // This class is a singleton
    static let shared = CellSpaceWatchAppManager()
    
    // The current active panel of the Watch App
    @Published var panel: CellSpaceWatchPanel = .none
    
    // There is only one default WatchConnectivity session
    var wcSession: WCSession!
    
    // We keep a reference of each watch panel's manager
    let biofeedbackWatchAppManager: BioFeedbackWatchAppManager = BioFeedbackWatchAppManager()
    
    override init() {
        super.init()
        if (WCSession.isSupported()) {
            wcSession = WCSession.default
            wcSession.delegate = self
            wcSession.activate()
        }
    }
    
    func switchPanel(panel: CellSpaceWatchPanel) {
        self.panel = panel
    }
}

// MARK: - WCSessionDelegate
extension CellSpaceWatchAppManager: WCSessionDelegate {
    
    func session(_ session: WCSession, activationDidCompleteWith activationState: WCSessionActivationState, error: Error?) {
        if (activationState == .activated) {
            print("Apple Watch's WCSession activated");
        } else {
            print("Apple Watch's WCSession activation failed");
        }
    }
    
    func sessionReachabilityDidChange(_ session: WCSession) {
        print("Apple Watch sessionReachabilityDidChange: \(session.isReachable)")
    }
    
    func session(_ session: WCSession, didReceiveApplicationContext applicationContext: [String : Any]) {
        // Switch to the corresponding panel after receiving a panel switch message
        if let watchPanelIndex = applicationContext["WatchPanel"] as? Int {
            if let watchPanel = CellSpaceWatchPanel(rawValue: watchPanelIndex) {
                if (self.panel != watchPanel) {
                    print("Switched to panel: \(String(describing: watchPanel))")
                    DispatchQueue.main.async {
                        self.panel = watchPanel
                    }
                }
            }
            return
        }
        
        if applicationContext["BioFeedback"] is Bool {
            biofeedbackWatchAppManager.didReceiveApplicationContext(applicationContext: applicationContext)
        }
    }
    
    func session(_ session: WCSession, didReceiveMessage message: [String : Any]) {
        if message["BioFeedback"] is Bool {
            biofeedbackWatchAppManager.didReceiveMessage(message: message)
            return
        }
    }
    
    func session(_ session: WCSession, didReceiveMessage message: [String : Any], replyHandler: @escaping ([String : Any]) -> Void) {
        if message["BioFeedback"] is Bool {
            biofeedbackWatchAppManager.didReceiveMessage(message: message, replyHandler: replyHandler)
            return
        }
    }
}
