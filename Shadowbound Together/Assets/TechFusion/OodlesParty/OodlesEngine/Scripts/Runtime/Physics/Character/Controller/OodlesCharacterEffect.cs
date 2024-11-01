﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OodlesParty
{
    public partial class OodlesCharacter : MonoBehaviour
    {
        [SerializeField]
        GameObject runningEffect;

        GameObject runEffInst;

        //todo OodlesEngine
        //[SyncVar(hook = nameof(SetRunningEffectVisible))]
        bool runningEffectVisible = false;
         
        void InitEffects()
        {
            if (runningEffect)
            {
                runEffInst = Instantiate(runningEffect, ragdollPlayer.transform);
                runEffInst.SetActive(false);
            }
            
        }

        public void SetRunningEffectVisible(bool oldVisibility, bool newVisibility)
        {
            if (runEffInst)
                runEffInst.SetActive(newVisibility);
        }
    }
}