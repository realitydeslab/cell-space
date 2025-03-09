//
//  CellSpaceAppView.swift
//  MockCellSpaceApp
//
//  Created by Yuchen Zhang on 2022/10/19.
//

import SwiftUI

struct CellSpaceAppView: View {
    
    @ObservedObject var cellspaceAppWatchConnectivityManager = MockCellSpaceAppWatchConnectivityManager.shared
    
    var body: some View {
        VStack {
            Text("CellSpace App")
            
            Spacer()
                .frame(height: 50)
            
            HStack {
                Button("Is Watch App Installed") {
                    print("Is watch app installed: \(cellspaceAppWatchConnectivityManager.isWatchAppInstalled())")
                }
                
                Text(": \(cellspaceAppWatchConnectivityManager.isWatchAppInstalledVar)" as String)
            }
            
            Spacer()
                .frame(height: 50)
            
            HStack {
                Button("Is Reachable") {
                    print("Is reachable: \(cellspaceAppWatchConnectivityManager.isReachable())")
                }
                
                Text(": \(cellspaceAppWatchConnectivityManager.isReachableVar)" as String)
            }
            
            Spacer()
                .frame(height: 50)
            
            Button("Play MOFA") {
                cellspaceAppWatchConnectivityManager.updatePanel(panelIndex: 1)
            }
        }
        .padding()
    }
}

struct CellSpaceAppView_Previews: PreviewProvider {
    static var previews: some View {
        CellSpaceAppView()
    }
}
