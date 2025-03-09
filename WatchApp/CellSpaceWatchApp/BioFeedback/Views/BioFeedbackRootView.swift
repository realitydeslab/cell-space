// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

import SwiftUI

struct BioFeedbackRootView: View {
    
    @ObservedObject var biofeedbackWatchAppManager = CellSpaceWatchAppManager.shared.biofeedbackWatchAppManager
    
    var body: some View {
        if (biofeedbackWatchAppManager.view == .readyView) {
            BioFeedbackReadyView()
        } else if (biofeedbackWatchAppManager.view == .handednessView) {
            BioFeedbackHandednessView()
        } else if (biofeedbackWatchAppManager.view == .fightingView) {
            BioFeedbackFightingView()
        } else if (biofeedbackWatchAppManager.view == .resultView) {
            BioFeedbackResultView()
        }
    }
}

struct BioFeedbackHomeView_Previews: PreviewProvider {
    static var previews: some View {
        BioFeedbackRootView()
    }
}
