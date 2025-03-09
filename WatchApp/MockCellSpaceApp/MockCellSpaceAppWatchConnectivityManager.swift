// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

import WatchConnectivity

enum CellSpaceWatchPanel: Int {
    case none = 0
    case biofeedback = 1
}

class MockCellSpaceAppWatchConnectivityManager: NSObject, ObservableObject {
    
    // This class is a singleton
    static let shared = MockCellSpaceAppWatchConnectivityManager()
    
    @Published var panel: CellSpaceWatchPanel = .none
    
    @Published var isWatchAppInstalledVar: Bool = false
    
    @Published var isReachableVar: Bool = false
    
    var wcSession: WCSession!
    
    let biofeedbackWatchConnectivityManager = MockBioFeedbackWatchConnectivityManager()
    
    override init() {
        super.init()
        if (WCSession.isSupported()) {
            wcSession = WCSession.default
            wcSession.delegate = self
            wcSession.activate()
        }
    }
    
    func hasPairedAppleWatch() -> Bool {
        return self.wcSession.isPaired;
    }
    
    func isWatchAppInstalled() -> Bool {
        isWatchAppInstalledVar = self.wcSession.isWatchAppInstalled
        return isWatchAppInstalledVar
    }
    
    func activate() {
        self.wcSession.activate()
    }
    
    func isReachable() -> Bool {
        isReachableVar = self.wcSession.isReachable
        return isReachableVar
    }
    
    func updatePanel(panelIndex: Int) {
        self.panel = CellSpaceWatchPanel(rawValue: panelIndex)!
        let context = ["WatchPanel" : panelIndex,
                       "Timestamp" : ProcessInfo.processInfo.systemUptime] as [String : Any];
        do {
            try self.wcSession.updateApplicationContext(context)
            print("Updated panel: \(panelIndex)")
        } catch {
            print("Failed to update panel")
        }
    }
}

extension MockCellSpaceAppWatchConnectivityManager: WCSessionDelegate {
    
    func session(_ session: WCSession, activationDidCompleteWith activationState: WCSessionActivationState, error: Error?) {
        if (activationState == .activated) {
            print("iPhone's WCSession activated");
            updatePanel(panelIndex: 0)
        } else {
            print("iPhone's WCSession activation failed")
        }
    }
    
    func sessionDidBecomeInactive(_ session: WCSession) {
        print("[iPhone] sessionDidBecomeInactive")
    }
    
    func sessionDidDeactivate(_ session: WCSession) {
        print("[iPhone] sessionDidDeactivate")
    }
    
    func sessionReachabilityDidChange(_ session: WCSession) {
        print("WCSession reachability did change: \(session.isReachable)")
    }
    
    func session(_ session: WCSession, didReceiveMessage message: [String : Any]) {
        if message["BioFeedback"] is Bool {
            biofeedbackWatchConnectivityManager.didReceiveMessage(message: message)
        }
    }
    
    func session(_ session: WCSession, didReceiveMessage message: [String : Any], replyHandler: @escaping ([String : Any]) -> Void) {

    }
}

