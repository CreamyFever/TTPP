using UnityEngine;

public class CPanelCollision : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Transform trans = other.gameObject.transform;

        if(trans.CompareTag("Panel"))
        {
            CPanel panel = trans.gameObject.GetComponent<CPanel>();


            panel.OnCrash();
        }
    }
}
