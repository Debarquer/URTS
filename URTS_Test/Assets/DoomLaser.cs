using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DoomLaserStages { Dormant, Stage1, Stage2, Stage3, Stage4, Firing}
public class DoomLaser : MonoBehaviour
{
    public DoomLaserStages doomLaserStage = DoomLaserStages.Dormant;

    public ParticleSystem[] stage1Effects;
    public ParticleSystem[] stage2Effects;
    public ParticleSystem[] stage3Effects;
    public ParticleSystem[] stage4Effects;
    public ParticleSystem[] neckEffects;
    public ParticleSystem[] firingEffects;
    public LineRenderer laser;

    public AudioSource audioSource;
    public AudioClip chargeUpClip;
    public AudioClip  loopingClip;
    public AudioClip  coolingdownClip;

    public void AdvanceStage() {
        switch (doomLaserStage) {
            case DoomLaserStages.Dormant:
                ActivateEffect(stage1Effects);
                audioSource.clip = chargeUpClip;
                audioSource.Play();
                Invoke("StartLoopingClip", 8f);
                doomLaserStage = DoomLaserStages.Stage1;
                break;
            case DoomLaserStages.Stage1:
                ActivateEffect(stage2Effects);
                audioSource.clip = chargeUpClip;
                audioSource.Play();
                Invoke("StartLoopingClip", 8f);
                doomLaserStage = DoomLaserStages.Stage2;
                break;
            case DoomLaserStages.Stage2:
                ActivateEffect(stage3Effects);
                audioSource.clip = chargeUpClip;
                audioSource.Play();
                Invoke("StartLoopingClip", 8f);
                doomLaserStage = DoomLaserStages.Stage3;
                break;
            case DoomLaserStages.Stage3:
                ActivateEffect(stage4Effects);
                audioSource.clip = chargeUpClip;
                audioSource.Play();
                Invoke("StartLoopingClip", 8f);
                doomLaserStage = DoomLaserStages.Stage4;
                break;
            case DoomLaserStages.Stage4:
                ActivateEffect(neckEffects);
                audioSource.clip = chargeUpClip;
                audioSource.Play();
                Invoke("StartLoopingClip", 8f);
                doomLaserStage = DoomLaserStages.Firing;
                break;
            case DoomLaserStages.Firing:
                ActivateEffect(firingEffects);
                laser.gameObject.SetActive(true);
                Invoke("StartLoopingClip", 8f);
                Invoke("DestroyHQ", 4f);
                break;
        }
    }

    void DestroyHQ() {
        Destroy(GameObject.Find("HQ"));
    }

    void ActivateEffect(ParticleSystem[] particleSystems) {
        foreach(ParticleSystem particleSystem in particleSystems) {
            particleSystem.gameObject.SetActive(true);
        }
    }

    void StartLoopingClip() {
        audioSource.clip = loopingClip;
        audioSource.loop = true;
        audioSource.Play();

        switch (doomLaserStage) {
            case DoomLaserStages.Dormant:
                Invoke("StartCooldownClip", 2f);
                break;
            case DoomLaserStages.Stage1:
                Invoke("StartCooldownClip", 4f);
                break;
            case DoomLaserStages.Stage2:
                Invoke("StartCooldownClip", 6f);
                break;
            case DoomLaserStages.Stage3:
                Invoke("StartCooldownClip", 8f);
                break;
            case DoomLaserStages.Stage4:
                Invoke("StartCooldownClip", 10f);
                break;
            case DoomLaserStages.Firing:
                Invoke("StartCooldownClip", 12f);
                break;
        }
    }

    void StartCooldownClip() {
        audioSource.clip = coolingdownClip;
        audioSource.Play();
        Invoke("StopClip", 8f);
    }

    void StopClip() {
        audioSource.Stop();
    }
}
