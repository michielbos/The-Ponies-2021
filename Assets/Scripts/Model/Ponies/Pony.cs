using System;
using System.Collections.Generic;
using Assets.Scripts.Controllers;
using Controllers.Playmode;
using JetBrains.Annotations;
using Model.Actions;
using Model.Actions.Actions;
using Model.Data;
using UnityEngine;

namespace Model.Ponies {

public class Pony: MonoBehaviour, ITimeTickListener, IActionProvider {
    public GameObject indicator;
    public Material indicatorMaterial;
    public List<PonyAction> queuedActions = new List<PonyAction>();
    [CanBeNull] public PonyAction currentAction;
    
    public string ponyName;
    public PonyRace race;
    public Gender gender;
    public PonyAge age;

    public bool IsSelected => HouseholdController.Instance.selectedPony == this;

    public void Init(string ponyName, PonyRace race, Gender gender, PonyAge age) {
        this.ponyName = ponyName;
        this.race = race;
        this.gender = gender;
        this.age = age;
        indicator.GetComponent<Renderer>().material = new Material(indicatorMaterial);
    }

    private void OnEnable() {
        TimeController.Instance.AddTickListener(this);
    }

    private void OnDisable() {
        TimeController.Instance.RemoveTickListener(this);
    }

    public void OnTick() {
        if (currentAction == null && queuedActions.Count > 0) {
            currentAction = queuedActions[0];
            currentAction.SetActive();
            UpdateQueue();
            
        }
        if (currentAction != null) {
            currentAction.Tick();
            if (currentAction.finished) {
                queuedActions.Remove(currentAction);
                currentAction = null;
                UpdateQueue();
            }
        }
    }

    public PonyData GetPonyData() {
        return new PonyData(ponyName, (int) race, (int) gender, (int) age);
    }

    public void SetSelected(bool selected) {
        indicator.SetActive(selected);
    }

    public void QueueAction(PonyAction action) {
        queuedActions.Add(action);
        UpdateQueue();
    }
    
    public List<PonyAction> GetActions(Pony pony) {
        if (pony == this) {
            return new List<PonyAction>() {
                new FakeAction(pony, "Why am I a cube?")
            };
        }
        return new List<PonyAction>() {
            new FakeAction(pony, "Chat"),
            new FakeAction(pony, "Slap"),
            new FakeAction(pony, "Flirt"),
        };
    }

    public void CancelAction(PonyAction action) {
        if (action == currentAction) {
            action.Cancel();
        } else {
            queuedActions.Remove(action);
        }
        UpdateQueue();
    }

    private void UpdateQueue() {
        if (IsSelected) {
            HouseholdController.Instance.actionQueue.UpdateActions(queuedActions);
        }
    }
}

}