﻿using UnityEngine;

public class ItemProjectileWeapon : ItemWeapon
{
    [Header("Projectile Weapon Data:")]
    [SerializeField]
    internal float _aimOffset;
    [SerializeField]
    internal LayerMask _hitMask;
    [Space(20)]
    [SerializeField]
    internal float fireDelay;
    [SerializeField]
    internal WeaponFireType _fireType;
    internal bool _aimed = false;
    internal bool _aiming = false;

    [SerializeField]
    internal bool _toggleAim;
    [Header("Audio:")]
    [Tooltip("The amount the pitch can be randomly adjusted on all sounds played by this weapon.")]
    [Range(0f, 0.3f)]
    public float pitchModAmount = 0.05f;
    [Tooltip("The maximum distance the fire sound can be heard from.")]
    [Range(25f, 500f)]
    public float fireHeardDistance = 150f;
    public AudioClip fireSound;


    private bool aimStarted = false;
    private float fireTime = 0f;
    private NetworkedPlayer player;
    private bool canFire = true;
    
    public override void InitFromNetwork(int netID)
    {
        base.InitFromNetwork(netID);
        fireTime = fireDelay;
    }

    public override void ActiveUpdate(int ownerID)
    {
        AimCheck(ownerID);
        FireCheck(ownerID);
    }

    private void FireCheck(int ownerID)
    {
        if(canFire)
        {
            if (_fireType == WeaponFireType.Full && Inp.Interact.Primary)
            {
                canFire = false;
                Fire(ownerID);
            }
            else if (_fireType == WeaponFireType.Semi && Inp.Interact.PrimaryDown)
            {
                canFire = false;
                Debug.Log("Fire");
                Fire(ownerID);
            }
        }else
        {
            fireTime += Time.deltaTime;
            if (fireTime >= fireDelay)
            {
                canFire = true;
                fireTime = 0f;
            }
        }
    }

    private void AimCheck(int ownerID)
    {
        if (_toggleAim)
        {
            if (Inp.Interact.SecondaryDown)
            {
                _aimed = !_aimed;
                Aim(ownerID);
            }
        }
        else if(Inp.Interact.Secondary != _aimed)
        {
            _aimed = Inp.Interact.Secondary;
            Aim(ownerID);
        }
    }

    public void Aim(int playerID)
    {
        Events.Player.SetAimOffset(playerID, _aimOffset);
        Events.Player.SetAnimatorBool(playerID, "aim", _aimed);
    }

    public void Fire(int playerID)
    {
        if (_aimed == false) return;

        SFX.PlayAt(fireSound, transform.position, transform, fireHeardDistance, pitchModAmount);

        Events.Player.SetAnimatorTrigger(playerID, "recoil_medium");
        
        RaycastHit hit;
        if(Physics.Raycast(PlayerMouseController.Instance.CenterScreenRay, out hit, _range, _hitMask))
        {
            FXController.HitAt(hit.point, hit.transform.GetComponent<IMaterialProperty>());
            NetworkedPlayer player = hit.transform.GetComponent<NetworkedPlayer>();
            if(player != null)
            {
                player.RequestDamageThisPlayer(Damage);
            }
        }
    }

    public override void Update()
    {
        
    }
    
}
