// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

import SwiftUI

struct BioFeedbackHandednessView: View {
    
    @ObservedObject var biofeedbackWatchAppManager = CellSpaceWatchAppManager.shared.biofeedbackWatchAppManager

    var body: some View {
        VStack {
            HStack {
                Button {
                    biofeedbackWatchAppManager.view = .readyView
                } label: {
                    Image("back")
                        .resizable()
                        .foregroundColor(.white)
                        .frame(maxWidth: 24, maxHeight: 24)
                }
                .buttonStyle(.plain)
                Spacer()
            }
            
            Text("The watch is on your")
                .font(Font.custom("ObjectSans-BoldSlanted", size: 13))
                .padding(.bottom)
            Spacer()
            rightHandButton
                .padding(.bottom)
            Spacer()
            leftHandButton
                .padding(.bottom)
        }
    }
    
    var rightHandButton: some View {
        Button {
            biofeedbackWatchAppManager.handedness = .right
        } label: {
            ZStack {
                Rectangle()
                    .frame(maxWidth: 120, maxHeight: 50)
                    .foregroundColor(biofeedbackWatchAppManager.handedness == .right ? .white : .black)
                    .border(Color.white)
                HStack {
                    Text("Right Hand")
                        .font(Font.custom("ObjectSans-BoldSlanted", size: 13))
                    Image("arrow-right")
                        .renderingMode(.template)
                        .resizable()
                        .frame(maxWidth: 16, maxHeight: 16)
                }
                .foregroundColor(biofeedbackWatchAppManager.handedness == .right ? .black : .white)
            }
        }
        .buttonStyle(.plain)
    }
    
    var leftHandButton: some View {
        Button {
            biofeedbackWatchAppManager.handedness = .left
        } label: {
            ZStack {
                Rectangle()
                    .frame(maxWidth: 120, maxHeight: 50)
                    .foregroundColor(biofeedbackWatchAppManager.handedness == .right ? .black : .white)
                    .border(Color.white)
                HStack {
                    Text("Left Hand")
                        .font(Font.custom("ObjectSans-BoldSlanted", size: 13))
                    Image("arrow-right")
                        .renderingMode(.template)
                        .resizable()
                        .frame(maxWidth: 16, maxHeight: 16)

                }
                .foregroundColor(biofeedbackWatchAppManager.handedness == .right ? .white : .black)

            }
        }
        .buttonStyle(.plain)
    }
}

struct BioFeedbackHandednessView_Previews: PreviewProvider {
    static var previews: some View {
        BioFeedbackHandednessView()
    }
}
