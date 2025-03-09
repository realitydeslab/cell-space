// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

import SwiftUI

struct BioFeedbackReadyView: View {
    
    @ObservedObject var biofeedbackWatchAppManager =
        CellSpaceWatchAppManager.shared.biofeedbackWatchAppManager
    
    var body: some View {
        VStack {
            HStack {
                Spacer()
                    .frame(width: 10)
                
                Button {
                    CellSpaceWatchAppManager.shared.switchPanel(panel: .none)
                } label: {
                    Image("back")
                        .resizable()
                        .foregroundColor(.white)
                        .frame(maxWidth: 24, maxHeight: 24)
                }
                .buttonStyle(.plain)
                Spacer()
                
                Button {
                    biofeedbackWatchAppManager.view = .handednessView
                } label: {
                    Image(systemName: "gearshape.fill")
                        .resizable()
                        .foregroundColor(.white)
                        .frame(maxWidth: 24, maxHeight: 24)
                }
                .buttonStyle(.plain)
                
                Spacer()
                    .frame(width: 10)
            }
            
            Spacer()
                .frame(height: 16)
            Text("Panel #001")
                .font(Font.custom("ObjectSans-Regular", size: 13))
                .padding()
            Text("BioFeedback")
                .font(Font.custom("ObjectSans-BoldSlanted", size: 18))
            Spacer()
            
            readyButton
        }
    }
    
    var readyButton: some View {
        Button {
            biofeedbackWatchAppManager.sendStartRoundMessage()
        } label: {
            ZStack {
                Rectangle()
                    .foregroundColor(.white)
                    .frame(maxWidth: 100, maxHeight: 40)
                
                Text("Ready")
                    .font(Font.custom("ObjectSans-BoldSlanted", size: 16))
                    .foregroundColor(.black)
            }
        }
        .buttonStyle(.plain)
    }
}

struct BioFeedbackIntroView_Previews: PreviewProvider {
    static var previews: some View {
        BioFeedbackReadyView()
    }
}
