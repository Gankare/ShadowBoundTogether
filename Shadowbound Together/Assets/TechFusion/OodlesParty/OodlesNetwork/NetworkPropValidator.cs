using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using OodlesParty;

[RequireComponent(typeof(NetworkIdentity))]
public class NetworkPropValidator : NetworkBehaviour
{
    protected void Awake()
    {
        //InitNetwork();

        NetworkIdentity id = GetComponent<NetworkIdentity>();
        if (id == null)
        {
            Debug.Log("NetworkPropValidator need a NetworkIdentity!");
            return;
        }

        if (!id.isServer)
        {
            this.enabled = false;

            Prop prop = GetComponent<Prop>();
            if (prop != null) { prop.enabled = false; }
        }
    }

    void InitNetwork()
    {
        NetworkIdentity id = gameObject.GetComponent<NetworkIdentity>();
        if (id == null)
        {
            id = gameObject.AddComponent<NetworkIdentity>();
        }

        NetworkedSingleTransform networkedSingleTransform = gameObject.GetComponent<NetworkedSingleTransform>();
        if (networkedSingleTransform == null)
        {
            networkedSingleTransform = gameObject.AddComponent<NetworkedSingleTransform>();
        }
        networkedSingleTransform._identity = id;
        networkedSingleTransform._transform = transform;

        NetworkedSingleTransformMessenger networkedSingleTransformMessenger = gameObject.GetComponent<NetworkedSingleTransformMessenger>();
        if (networkedSingleTransformMessenger == null)
        {
            networkedSingleTransformMessenger = gameObject.AddComponent<NetworkedSingleTransformMessenger>();
        }

        SingleTransformPrediction singleTransformPrediction = gameObject.GetComponent<SingleTransformPrediction>();
        if (singleTransformPrediction == null)
        {
            singleTransformPrediction = gameObject.AddComponent<SingleTransformPrediction>();
        }
    }
}
