using UnityEngine;
using System.Collections;
using Brawler.Combat;
using Brawler.Fighter;

public class Robot : FighterBase {

    public override string FighterName => "Toy Robot";

    private MeterController meter;
    private AttackController attacks;
    [SerializeField] private AttackData longDistanceGrabData;

    protected override void OnFighterInitialized() {
        meter = GetComponent<MeterController>();
        attacks = GetComponent<AttackController>();
        attacks.OnHeavyAttackPressed += OnHeavyAttackPressed;
        attacks.OnSpecialPressed += OnSpecialPressed;
    }

    protected override void OnTakeDamage(float damage) {
        meter.AddMeter(damage);
    }

    private void OnHeavyAttackPressed() {
        attacks.TryAttack(AttackContext.Neutral);
    }

    private void OnSpecialPressed() {
        if (meter.HasEnoughMeter(meter.maxMeter)) {
            meter.ConsumeMeter(meter.maxMeter);
            TriggerLongDistanceGrab();
        }
    }

    private void TriggerLongDistanceGrab() {
        attacks.ForceGrab(longDistanceGrabData);
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        attacks.OnHeavyAttackPressed -= OnHeavyAttackPressed;
        attacks.OnSpecialPressed -= OnSpecialPressed;
    }
}