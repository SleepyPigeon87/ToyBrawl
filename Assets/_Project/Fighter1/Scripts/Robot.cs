using UnityEngine;
using System.Collections;
using Brawler.Combat;
namespace Brawler.Fighter {
    public class Robot : FighterBase {
        public override string FighterName => "Toy Robot";
        private MeterController meter;
        private AttackController attacks;
        [SerializeField] private AttackData longDistanceGrabData;
        [SerializeField] private AttackData lightAttackData;
        [SerializeField] private float lungeForce = 5f;
        [SerializeField] private AttackData dodgeAttackData;
        [SerializeField] private float dodgeForce = 4f;

        protected override void OnFighterInitialized() {
            meter = GetComponent<MeterController>();
            attacks = GetComponent<AttackController>();
            attacks.OnAttackStarted += OnAttackStarted;
            attacks.OnLightAttackPressed += OnLightAttackPressed;
            attacks.OnHeavyAttackPressed += OnHeavyAttackPressed;
            attacks.OnSpecialPressed += OnSpecialPressed;
        
        }

        protected override void OnRespawn(Vector2 position) {
            StartCoroutine(RespawnCoroutine());
        
        }

        private IEnumerator RespawnCoroutine() {
            yield return new WaitForSeconds(2f);
            EndRespawnInvincibility();
        
        }

        protected override void OnTakeDamage(float damage) {
            if (meter == null) {
                Debug.Log("[Robot] meter is null in OnTakeDamage!");
                return;
            }
            meter.AddMeter(damage);
        }

        public override void OnAttackHit(FighterBase opponent, AttackData attack) {
            base.OnAttackHit(opponent, attack);
            if (meter == null) return;
            meter.AddMeter(attack.damage * 0.3f);

        }

        private void OnAttackStarted(AttackData attack) {
            if (attack == lightAttackData) {
                Rb.AddForce(new Vector2(FacingDirection * lungeForce, 0f), ForceMode2D.Impulse);
        
            }

            if (attack == dodgeAttackData) {
                float dodgeDir = Input.MoveInput.x;
                if (Mathf.Abs(dodgeDir) < 0.1f) dodgeDir = -FacingDirection;
                Rb.AddForce(new Vector2(dodgeDir * dodgeForce, 0f), ForceMode2D.Impulse);

            }

        }
        private void OnLightAttackPressed() {
            attacks.TryAttack(AttackContext.Neutral);

        }

        private void OnHeavyAttackPressed() {
            attacks.TryAttack(AttackContext.Forward);

        }

        private void OnSpecialPressed() {
            if (meter.HasEnoughMeter(meter.maxMeter)) {
                meter.ConsumeMeter(meter.maxMeter);
                Input.TriggerRumble(0.5f, 1f, 0.4f);
                TriggerLongDistanceGrab();
        
            }
        
        }

        private void TriggerLongDistanceGrab() {
            attacks.ForceGrab(longDistanceGrabData);
        
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            if (attacks != null) {
                attacks.OnLightAttackPressed -= OnLightAttackPressed;
                attacks.OnHeavyAttackPressed -= OnHeavyAttackPressed;
                attacks.OnSpecialPressed -= OnSpecialPressed;
                attacks.OnAttackStarted -= OnAttackStarted;

            }

        }

    }

}