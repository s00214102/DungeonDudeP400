using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Action_Attack : Action_Base
{
	List<System.Type> SupportedGoals = new List<System.Type> { typeof(Goal_Attack) };
	Health enemyHealth = null;
	//TODO bad place to put these variables?
	public GameObject projectilePrefab;
	private float projectileMoveSpeed = 15f;
	public UnityEvent AttackFinished;
	private void OnAttackFinished() { AttackFinished?.Invoke(); }

	public override List<System.Type> GetSupportedGoals()
	{
		return SupportedGoals;
	}
	public override float GetCost()
	{
		return 0f;
	}
	public override void OnActivated(Goal_Base _linkedGoal)
	{
		goap_debug.ChangeActionImage(5);
		base.OnActivated(_linkedGoal);

		var result = knowledge.RecallHighestAlertEnemy();
		if (result.found)
			InvokeRepeating("AttackTarget", 0, data.HeroData.AttackRate);
	}
	public override void OnDeactived()
	{
		base.OnDeactived();
		CancelInvoke("AttackTarget");
		enemyHealth = null;
	}
    public override void OnTick()
    {
        // face target
		var result = knowledge.RecallHighestAlertEnemy();
		if (result.found)
			this.gameObject.transform.rotation = Helper.RotateToFaceTargetOverTime(this.gameObject.transform, result.enemy.transform, 5f);
    }
    private void AttackTarget()
	{
		if (enemyHealth == null)
			{
				var result = knowledge.RecallHighestAlertEnemy();
				enemyHealth = result.enemy.GetComponent<Health>();
				}
		if (enemyHealth != null && !enemyHealth.isDead)
		{
			GameObject sphere = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
			sphere.GetComponent<Projectile>().targetPosition = enemyHealth.transform.position;

			enemyHealth.TakeDamage(data.HeroData.Damage);
			OnAttackFinished();
		}
	}
}