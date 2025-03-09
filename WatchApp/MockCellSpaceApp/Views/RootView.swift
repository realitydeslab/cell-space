import SwiftUI

struct RootView: View {
    
    @ObservedObject var cellspaceAppWatchConnectivityManager = MockCellSpaceAppWatchConnectivityManager.shared
    
    var body: some View {
        ZStack {
            if (cellspaceAppWatchConnectivityManager.panel == .none) {
                CellSpaceAppView()
            } else if (cellspaceAppWatchConnectivityManager.panel == .biofeedback) {
                BioFeedbackView()
            }
        }
        .onDisappear {
            print("RootView onDisappear")
            cellspaceAppWatchConnectivityManager.updatePanel(panelIndex: 0)
        }
    }
}

struct ContentView_Previews: PreviewProvider {
    static var previews: some View {
        RootView()
    }
}
