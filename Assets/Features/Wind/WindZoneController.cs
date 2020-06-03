using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WindZone))]
public class WindZoneController : MonoBehaviour
{
    private WindZone windZone;
    public WindManager windManager;
    // Start is called before the first frame update
    void Start()
    {
        windZone = this.GetComponent<WindZone>();
    }

    // Update is called once per frame
    void Update()
    {
        windZone.windMain = windManager.WindStrength;
    }
}
