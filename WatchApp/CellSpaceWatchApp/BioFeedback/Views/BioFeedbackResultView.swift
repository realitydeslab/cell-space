// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

import SwiftUI
import Foundation

struct BioFeedbackResultView: View {
    
    @ObservedObject var biofeedbackWatchAppManager = CellSpaceWatchAppManager.shared.biofeedbackWatchAppManager
    
    @State var result: Bool = true
    
    var body: some View {
        VStack {
            resultText
            //Spacer()
            dataList
                .padding(.top, 5)
                .padding(.bottom, 5)
            playAgainButton
        }
    }
    
    var resultText: some View {
        var resultText: Text
        if (self.biofeedbackWatchAppManager.roundResult == .victory) {
            resultText = Text("Victory")
        } else if (self.biofeedbackWatchAppManager.roundResult == .defeat) {
            resultText = Text("Defeat")
        } else {
            resultText = Text("Draw")
        }
        return resultText
            .font(Font.custom("ObjectSans-Bold", size: 20))
    }
    
    var dataList: some View {
        HStack {
            VStack (alignment: .leading, spacing: 5) {
                Text("Kills: \(self.biofeedbackWatchAppManager.kill)")
                Text("Hit Rate: \(Int(self.biofeedbackWatchAppManager.hitRate * 100))%")
                Text("Dist: \(Int(self.biofeedbackWatchAppManager.distance * self.biofeedbackWatchAppManager.meterToFoot)) ft")
                Text("Energy: \(Int(self.biofeedbackWatchAppManager.activeEnergy)) kcal")
            }
            .font(Font.custom("ObjectSans-BoldSlanted", size: 12))
            
            Spacer()
            
            BioFeedbackActivityRingsView(healthStore: self.biofeedbackWatchAppManager.healthStore)
                .frame(width: 40, height: 40)
        }
        .padding(.horizontal, 8)
    }
    
    var playAgainButton: some View {
        Button {
            self.biofeedbackWatchAppManager.view = .readyView
        } label: {
            HStack {
                Text("Play Again")
                    .font(Font.custom("ObjectSans-BoldSlanted", size: 14))
                Image("arrow-right")
                    .renderingMode(.template)
                    .resizable()
                    .frame(maxWidth: 10, maxHeight: 10)
            }
            .font(.headline)
            .fontWeight(.semibold)
            .padding(6)
            .foregroundColor(.black)
            .background(Color.white)
        }
        .buttonStyle(.plain)
    }
}

struct BioFeedbackResultView_Previews: PreviewProvider {
    static var previews: some View {
        BioFeedbackResultView().environmentObject(BioFeedbackWatchAppManager())
    }
}
