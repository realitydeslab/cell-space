//
//  MofaView.swift
//  MockHoloKitApp
//
//  Created by Yuchen Zhang on 2022/10/19.
//

import SwiftUI

struct BioFeedbackView: View {
    
    @ObservedObject var biofeedbackWatchConnectivityManager = MockCellSpaceAppWatchConnectivityManager.shared.biofeedbackWatchConnectivityManager
    
    var body: some View {
        VStack {
            Text("BioFeedback")
            
            Spacer()
                .frame(height: 50)
            
            Button("Start Round") {
                biofeedbackWatchConnectivityManager.onRoundStarted()
            }
            
            Spacer()
                .frame(height: 50)
            
            Button("End Round") {
                biofeedbackWatchConnectivityManager.onRoundEnded(roundResult: .victory, kill: 8, hitRate: 24)
            }
            
            Spacer()
                .frame(height: 50)
            
            Button("Load Homepage") {
                MockCellSpaceAppWatchConnectivityManager.shared.updatePanel(panelIndex: 0)
            }
        }
    }
}

struct BiofeedbackView_Previews: PreviewProvider {
    static var previews: some View {
        BioFeedbackView()
    }
}
