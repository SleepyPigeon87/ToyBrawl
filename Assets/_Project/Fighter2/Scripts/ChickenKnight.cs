using UnityEngine;
using System.Collections;
using Brawler.Combat;
namespace Brawler.Fighter {
    public class ChickenKnight : FighterBase {

        public override string FighterName => "Chicken Knight";

        //Heavy Attack Lunge
        [SerializeField] private AttackData forwardAttackData;
        [SerializeField] private float lungeForce = 10f;
        [SerializeField] private AttackData dodgeAttackData;
        [SerializeField] private float dodgeForce = 8f;
        private MeterController meter;
        private AttackController attacks;

        protected override void OnFighterInitialized() {
            Debug.Log("[ChickenKnight] OnFighterInitialized called!");
            meter = GetComponent<MeterController>();
            attacks = GetComponent<AttackController>();
            Debug.Log($"[ChickenKnight] meter={meter} attacks={attacks}");
            attacks.OnLightAttackPressed += OnLightAttackPressed;
            attacks.OnHeavyAttackPressed += OnHeavyAttackPressed;
            attacks.OnAttackStarted += OnAttackStarted;
            attacks.OnSpecialPressed += OnSpecialPressed;
        }

        protected override void OnRespawn(Vector2 position) {
            StartCoroutine(RespawnCoroutine());
        }

        private IEnumerator RespawnCoroutine() {
            yield return new WaitForSeconds(2f);
            EndRespawnInvincibility();
        }

        public override void OnAttackHit(FighterBase opponent, AttackData attack) {
            base.OnAttackHit(opponent, attack);
            meter.AddMeter(attack.damage);
        }

        private void OnLightAttackPressed() {
            Debug.Log("[ChickenKnight] OnLightAttackPressed received!");
            attacks.TryAttack(AttackContext.Neutral);
        }

        private void OnHeavyAttackPressed() {
            attacks.TryAttack(AttackContext.Forward);
        }

        private void OnAttackStarted(AttackData attack) {
            if (attack == forwardAttackData) {
                Rb.AddForce(new Vector2(FacingDirection * lungeForce, 0f), ForceMode2D.Impulse);
            }

            if (attack == dodgeAttackData) {
                float dodgeDir = Input.MoveInput.x;
                if (Mathf.Abs(dodgeDir) < 0.1f) dodgeDir = -FacingDirection;
                Rb.AddForce(new Vector2(dodgeDir * dodgeForce, 0f), ForceMode2D.Impulse);
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
                attacks.OnLightAttackPressed -= OnLightAttackPressed;
                attacks.OnHeavyAttackPressed -= OnHeavyAttackPressed;
                attacks.OnAttackStarted -= OnAttackStarted;
                attacks.OnSpecialPressed -= OnSpecialPressed;
            } else {
                Debug.Log("Its null!");

            }
        }
    }

}