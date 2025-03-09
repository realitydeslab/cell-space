// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

import SwiftUI

struct RootView: View {
    
    @ObservedObject var cellspaceWatchAppManager = CellSpaceWatchAppManager.shared
    
    var body: some View {
        if (cellspaceWatchAppManager.panel == .none) {
            PanelListView()
        } else if (cellspaceWatchAppManager.panel == .biofeedback) {
            BioFeedbackRootView()
                .onAppear {
                    cellspaceWatchAppManager.biofeedbackWatchAppManager.onAppear()
                }
                .onDisappear {
                    cellspaceWatchAppManager.biofeedbackWatchAppManager.OnDisappear()
                }
        }
    }
    
    var defaultIntroView: some View {
        VStack {
            Image("cellspace-icon")
                .resizable()
                .frame(maxWidth: 120, maxHeight: 120)
                .padding(.bottom)
            Text("CellSpace")
                .font(Font.custom("ObjectSans-BoldSlanted", size: 16))
        }
    }
}

struct HomeView_Previews: PreviewProvider {
    static var previews: some View {
        RootView()
    }
}
