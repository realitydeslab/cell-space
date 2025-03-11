// SPDX-FileCopyrightText: Copyright 2023 Reality Design Lab <dev@reality.design>
// SPDX-FileContributor: Botao Amber Hu <botao.a.hu@gmail.com>
// SPDX-License-Identifier: MIT

using UnityEngine;
using System.Linq;
using VoroGen;
using HoloKit.ColocatedMultiplayerBoilerplate;

namespace CellSpace {

    [RequireComponent(typeof(VoronoiGenerator))]
    public class AssigningPlayerAndObjectAsVoronoiSite : MonoBehaviour 
    {
        public GameObject target;
        // Update is called once per frame
        void Update()
        {
            if (target != null) {
                GetComponent<VoronoiGenerator>().Sites 
                    = FindObjectsOfType<PlayerRole>()?.Select(p => p.gameObject)
                        .Concat(target.transform.Cast<Transform>()?.Select(p => p.gameObject)).ToArray();
            } else {
                GetComponent<VoronoiGenerator>().Sites 
                    = FindObjectsOfType<PlayerRole>()?.Select(p => p.gameObject).ToArray();
            }        
        }
    }
}
