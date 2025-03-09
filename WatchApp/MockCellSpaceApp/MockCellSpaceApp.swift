// SPDX-FileCopyrightText: Copyright 2024 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Yuchen Zhang <yuchen@reality.design>
// SPDX-License-Identifier: MIT

import SwiftUI

@main
struct MockCellSpaceApp: App {
    
    @State var cellspaceAppWatchConnectivityManager = MockCellSpaceAppWatchConnectivityManager()
    
    @State var biofeedbackWatchConnectivityManager = MockBioFeedbackWatchConnectivityManager()
    
    var body: some Scene {
        WindowGroup {
            RootView()
                .environmentObject(cellspaceAppWatchConnectivityManager)
                .environmentObject(biofeedbackWatchConnectivityManager)
        }
    }
}
