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
    private const float WALK_SPEED = 3f;
    
    public GameObject indicator;
    public Material indicatorMaterial;
    public List<PonyAction> queuedActions = new List<PonyAction>();
    [CanBeNull] public PonyAction currentAction;
    
    public string ponyName;
    public PonyRace race;
    public Gender gender;
    public PonyAge age;

    [CanBeNull] private Path walkPath;
    private Vector2Int? nextWalkTile;

    public bool IsSelected => HouseholdController.Instance.selectedPony == this;
    public bool IsWalking => walkPath != null;
    
    public Vector2Int TilePosition {
        get {
            Vector3 position = transform.position;
            return new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));
        }
        set { transform.position = new Vector3(value.x, 0, value.y); }
    }

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
        if (TimeController.HasInstance) {
            TimeController.Instance.RemoveTickListener(this);
        }
    }

    private void Update() {
        HandleWalking();
    }

    private void HandleWalking() {
        if (!IsWalking)
            return;
        if (nextWalkTile == null) {
            nextWalkTile = walkPath.NextTile();
        }
        Vector2Int nextTile = nextWalkTile.Value;
        Vector3 target = new Vector3(nextTile.x, 0, nextTile.y);
        float speed = WALK_SPEED * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target, speed);
        if (transform.position == target) {
            nextWalkTile = null;
            if (!walkPath.HasNext()) {
                walkPath = null;
            }
        }
    }

    public void OnTick() {
        if (currentAction == null && queuedActions.Count > 0) {
            currentAction = queuedActions[0];
            currentAction.SetActive();
            UpdateQueue();
            
        }
        if (currentAction != null) {
            currentAction.TickAction();
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
        if (IsSelected) {
            SoundController.Instance.PlaySound(SoundType.QueueAdded);
            UpdateQueue();
        }
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
        if (IsSelected) {
            SoundController.Instance.PlaySound(SoundType.QueueRemoved);
            UpdateQueue();
        }
    }

    private void UpdateQueue() {
        if (IsSelected) {
            HouseholdController.Instance.actionQueue.UpdateActions(queuedActions);
        }
    }

    public bool SetWalkTarget(Vector2Int target) {
        walkPath = Pathfinding.PathToTile(TilePosition, target);
        return walkPath != null;
    }
}

}