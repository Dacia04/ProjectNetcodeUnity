using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Transform spawnedObjectPrefab;
    private Transform spawnedObjectTransform;

    private NetworkVariable<MyCustomData> randomNumber = new(new MyCustomData(56,true,""), NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);

    public struct MyCustomData : INetworkSerializable
    {
        public int _int;
        public bool _bool;
        public FixedString128Bytes _message;
        public MyCustomData(int _int,bool _bool,string _message)
        {
            this._int = _int;
            this._bool = _bool;
            this._message = _message;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _bool);
            serializer.SerializeValue(ref _message);
        }
    }

    public override void OnNetworkSpawn()
    {
        randomNumber.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) =>
        {
            Debug.Log(OwnerClientId + "; randomNumber" + newValue._int + "; " + newValue._bool + "; " + newValue._message);
        };
    }
    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.T))
        {
            spawnedObjectTransform = Instantiate(spawnedObjectPrefab);
            spawnedObjectTransform.GetComponent<NetworkObject>().Spawn(true); // Only server can spawn object
            //TestServerRpc();
            //TestClientRpc(new ClientRpcParams { Send = new ClientRpcSendParams {TargetClientIds = new List<ulong>{1}}}); // only client have sendID execute fuction
            //randomNumber.Value = new MyCustomData(10, false,"All your base are belong to us");
        }
        if(Input.GetKeyDown(KeyCode.Y))
        {
            spawnedObjectTransform.GetComponent<NetworkObject>().Despawn(true); 
            //Destroy(spawnedObjectTransform.gameObject); // only server can destroy object
        }

        Vector3 moveDir = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W)) moveDir.z = +1f;
        if (Input.GetKey(KeyCode.S)) moveDir.z = -1f;
        if (Input.GetKey(KeyCode.A)) moveDir.x = -1f;
        if (Input.GetKey(KeyCode.D)) moveDir.x = +1f;

        transform.position += moveDir * moveSpeed * Time.deltaTime;

    }

    [ServerRpc] // click on both client and server   but only run on server
    private void TestServerRpc(ServerRpcParams serverRpcParams)
    {
        Debug.Log("Test Server RPC " + OwnerClientId + "; " + serverRpcParams.Receive.SenderClientId);
    }

    [ClientRpc] // click on server and     run on server, then send signal to run on client
    private void TestClientRpc(ClientRpcParams clientRpcParams)
    {
        Debug.Log("Test Client RPC" );
    }
}
