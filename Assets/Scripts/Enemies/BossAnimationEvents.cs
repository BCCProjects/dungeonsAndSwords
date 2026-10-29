using UnityEngine;

// O Animator do chefe fica no filho "Root" do esqueleto do personagem.
// Os Animation Events chegam aqui e são repassados para o BossController.
public class BossAnimationEvents : MonoBehaviour
{
    private BossController boss;

    private void Awake() => boss = GetComponentInParent<BossController>();

    public void AttackHit() => boss.AttackHit();
    public void ShootBeam() => boss.ShootBeam();
}
