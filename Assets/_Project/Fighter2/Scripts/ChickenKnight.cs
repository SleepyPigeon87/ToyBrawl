using UnityEngine;
using System.Collections;
using Brawler.Combat;
using Brawler.Fighter;

public class ChickenKnight : FighterBase {

    public override string FighterName => "Chicken Knight";

    //Heavy Attack Lunge
    [SerializeField] private AttackData forwardAttackData;
    [SerializeField] private float lungeForce = 10f;
    private MeterController meter;
    private AttackController attacks;

    protected override void OnFighterInitialized() {
        meter = GetComponent<MeterController>();
        attacks = GetComponent<AttackController>();
        attacks.OnHeavyAttackPressed += OnHeavyAttackPressed;
        attacks.OnAttackStarted += OnAttackStarted;
        attacks.OnSpecialPressed += OnSpecialPressed;
    }

    public override void OnAttackHit(FighterBase opponent, AttackData attack) {
        base.OnAttackHit(opponent, attack);
        meter.AddMeter(attack.damage);
    }

    private void OnHeavyAttackPressed() {
        attacks.TryAttack(AttackContext.Forward);
    }

    private void OnAttackStarted(AttackData attack) {
        if (attack == forwardAttackData) {
            Rb.AddForce(new Vector2(FacingDirection * lungeForce, 0f), ForceMode2D.Impulse);
        }
    }

    private void OnSpecialPressed() {
        if (meter.HasEnoughMeter(meter.maxMeter)) {
            meter.ConsumeMeter(meter.maxMeter);
            TriggerSpeedSlash();
        }
    }

    private void TriggerSpeedSlash() {
        StartCoroutine(SpeedSlashCoroutine());
    }

    private IEnumerator SpeedSlashCoroutine() {
        attacks.speedMultiplier = 0.3f;
        yield return new WaitForSeconds(5f);
        attacks.speedMultiplier = 1f;
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        if (attacks != null) {
            attacks.OnHeavyAttackPressed -= OnHeavyAttackPressed;
            attacks.OnAttackStarted -= OnAttackStarted;
            attacks.OnSpecialPressed -= OnSpecialPressed;
        } else {
            Debug.Log("Its null!");

        }
    }
}