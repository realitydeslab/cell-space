// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

import WatchConnectivity
    
enum BioFeedbackRoundResult: Int {
    case victory = 0
    case defeat = 1
    case draw = 2
}

class MockBioFeedbackWatchConnectivityManager: NSObject, ObservableObject {
    
    var isFighting = false
    
    override init() {
        super.init()
    }
    
    func onRoundStarted() {
        let context = ["BioFeedback" : true,
                       "Start" : true,
                       "MagicSchool" : 4,
                       "Timestamp" : ProcessInfo.processInfo.systemUptime] as [String : Any]
        
        do {
            try MockCellSpaceAppWatchConnectivityManager.shared.wcSession.updateApplicationContext(context)
            self.isFighting = true
            print("Fighting phase synced")
        } catch {
            print("Failed to sync fighting phase")
        }
    }
    
    func onRoundEnded(roundResult: BioFeedbackRoundResult, kill: Int, hitRate: Float) {
        let context = ["BioFeedback" : true,
                       "End" : true,
                       "RoundResult" : roundResult.rawValue,
                       "Kill" : kill,
                       "HitRate" : hitRate] as [String : Any]
        do {
            try MockCellSpaceAppWatchConnectivityManager.shared.wcSession.updateApplicationContext(context)
            self.isFighting = false
            print("Round result synced")
        } catch {
            print("Failed to sync round result")
        }
    }
    
    func queryWatchState() {
        let message = ["BioFeedback" : true,
                       "QueryState" : 0] as [String : Any]
        MockCellSpaceAppWatchConnectivityManager.shared.wcSession.sendMessage(message) { replyMessage in
            if let watchStateIndex = replyMessage["WatchState"] as? Int {
                print("On received query watch state replay: \(watchStateIndex)")
            }
        }
    }
    
    func didReceiveMessage(message: [String : Any]) {
        if message["Start"] is Bool {
            onRoundStarted()
        }
    }
}
