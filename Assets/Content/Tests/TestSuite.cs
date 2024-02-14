using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class TestSuite
{
    private GameObject towerDefenceGame;
    private GameManager gameManager;
    private BankManager bankManager;
    private GameObject tower;

    [SetUp]
    public void Setup()
    {
        towerDefenceGame = Object.Instantiate(Resources.Load<GameObject>("Prefabs/TowerDefenceGame"));
        gameManager = towerDefenceGame.GetComponentInChildren<GameManager>();
        bankManager = towerDefenceGame.GetComponentInChildren<BankManager>();
        tower = towerDefenceGame.GetComponentInChildren<TowerController>().gameObject;
    }

    [TearDown]
    public void Teardown()
    {
        Object.Destroy(towerDefenceGame.gameObject);
    }

    [UnityTest]
    public IEnumerator EnemySpawner_OneSecondAfterGameStart_AtLeastOneEnemyHasSpawned()
    {
        gameManager.StartGame();
        EnemySpawner enemySpawner = towerDefenceGame.GetComponentInChildren<EnemySpawner>();

        yield return new WaitForSeconds(1);

        Assert.Less(enemySpawner.GetNumEnemiesLeftToSpawn(), enemySpawner.numEnemiesToSpawn);

        gameManager.EndGame(); //
        gameManager.RestartGame(); //TODO remove these once enemies are parented to terrain on spawn (currently necessary to remove enemies from battlefield for next test)
    }

    [UnityTest]
    public IEnumerator EnemyNextToTower_AfterSeveralSeconds_TowerHealthHasDecreased()
    {
        GameObject enemy = Object.Instantiate(Resources.Load<GameObject>("Prefabs/EnemyRagdoll"));
        float towerInitialHealth = tower.GetComponent<Health>().GetCurrentHealth();
        Vector3 towerPosition = tower.transform.position;
        enemy.transform.position =  towerPosition + new Vector3(0, 0, 0.1f);
        enemy.GetComponent<EnemyController>().OnSpawn();

        yield return new WaitForSeconds(4);

        Assert.Less(tower.GetComponent<Health>().GetCurrentHealth(), towerInitialHealth);

        Object.Destroy(enemy);
    }

    [UnityTest]
    public IEnumerator EnemyAndLaserTurret_AfterOneSecond_EnemyHealthHasDecreased()
    {
        GameObject enemy = Object.Instantiate(Resources.Load<GameObject>("Prefabs/EnemyRagdoll"));
        enemy.transform.position = new Vector3(0.1f, 0, 0.1f);
        enemy.GetComponent<EnemyController>().OnSpawn();
        float enemyInitialHealth = enemy.GetComponent<Health>().GetCurrentHealth();
        GameObject laserTurret = Object.Instantiate(Resources.Load<GameObject>("Prefabs/LaserTurret"));
        laserTurret.transform.position = new Vector3(-0.1f, 0, 0.1f);
        laserTurret.transform.parent = tower.transform;
        laserTurret.transform.localScale = new Vector3(1, 1, 1);

        yield return new WaitForSeconds(1f);

        Assert.Less(enemy.GetComponent<Health>().GetCurrentHealth(), enemyInitialHealth);

        Object.Destroy(enemy);
        Object.Destroy(laserTurret);
    }

    [UnityTest]
    public IEnumerator Enemy_OnEnemyDeath_CreditBalancedHasIncreased()
    {
        int initialCreditBalance = bankManager.GetCreditBalance();
        GameObject enemy = Object.Instantiate(Resources.Load<GameObject>("Prefabs/EnemyRagdoll"));
        enemy.transform.position = new Vector3(0.1f, 0, 0.1f);
        enemy.GetComponent<EnemyController>().OnSpawn();

        yield return new WaitForSeconds(0.1f);

        enemy.GetComponent<EnemyController>().OnDeath();
        Assert.Greater(bankManager.GetCreditBalance(), initialCreditBalance);

        Object.Destroy(enemy);
    }

    [UnityTest]
    public IEnumerator GameStarted_OnTurretSelectAndPlace_CreditBalanceHasDecreased()
    {
        bankManager.ModifyCreditBalance(100);
        int initialCreditBalance = bankManager.GetCreditBalance();
        TurretSlotController turretSlot = Object.FindObjectOfType<TurretSlotController>();

        gameManager.turretPlacementController.OnTurretIconSelected(Resources.Load<GameObject>("Prefabs/LaserTurret"));
        gameManager.turretPlacementController.RegisterSlotSelection(turretSlot);

        yield return new WaitForSeconds(0.1f);

        Assert.Less(bankManager.GetCreditBalance(), initialCreditBalance);
    }

    [UnityTest]
    public IEnumerator EnemyNextToTower_AfterLaserBoltWeaponHasFiredAtEnemyPosition_EnemyHealthHasDecreased()
    {
        GameObject enemy = Object.Instantiate(Resources.Load<GameObject>("Prefabs/EnemyRagdoll"));
        float enemyInitialHealth = enemy.GetComponent<Health>().GetCurrentHealth();
        enemy.transform.position = tower.transform.position + new Vector3(0, 0, 0.2f);
        enemy.GetComponent<EnemyController>().OnSpawn();

        tower.GetComponentInChildren<IWeapon>().FireWeaponAt(enemy.transform.position);

        yield return new WaitForSeconds(1);

        Assert.Less(enemy.GetComponent<Health>().GetCurrentHealth(), enemyInitialHealth);

        Object.Destroy(enemy);
    }

    [UnityTest]
    public IEnumerator Tower_TowerTakesFatalDamage_GameEnds()
    {
        float initialTowerHealth = tower.GetComponent<Health>().GetCurrentHealth();
        tower.GetComponent<TowerController>().TakeDamage((int)initialTowerHealth);

        yield return new WaitForSeconds(1f);

        Assert.AreEqual(GameManager.GameState.End, gameManager.GetGameState());
    }

    [UnityTest]
    public IEnumerator TurretXAndTurretYInLine_OnEnemySpawnInLineBehindTurretY_TurretXDoesNotShootTurretY()
    {
        GameObject laserTurret = Object.Instantiate(Resources.Load<GameObject>("Prefabs/LaserTurret"));
        laserTurret.transform.position = new Vector3(0, 0, 0.2f);
        laserTurret.transform.parent = tower.transform;
        laserTurret.transform.localScale = new Vector3(1, 1, 1);

        GameObject secondLaserTurret = Object.Instantiate(Resources.Load<GameObject>("Prefabs/LaserTurret"));
        secondLaserTurret.transform.position = new Vector3(0f, 0, 0.4f);
        secondLaserTurret.transform.parent = tower.transform;
        secondLaserTurret.transform.localScale = new Vector3(1, 1, 1);
        float secondTurretInitialHealth = secondLaserTurret.GetComponent<Health>().GetCurrentHealth();

        GameObject enemy = Object.Instantiate(Resources.Load<GameObject>("Prefabs/EnemyRagdoll"));
        enemy.transform.position = new Vector3(0, 0, 0.6f);
        enemy.GetComponent<EnemyController>().OnSpawn();

        yield return new WaitForSeconds(1f);

        Assert.AreEqual(secondLaserTurret.GetComponent<Health>().GetCurrentHealth(), secondTurretInitialHealth);

        Object.Destroy(enemy);
        Object.Destroy(laserTurret);
        Object.Destroy(secondLaserTurret);
    }

    [UnityTest]
    public IEnumerator EnemyAndProjectileLauncherTurret_AfterSixSecondsSecond_EnemyHealthHasDecreased()
    {
        GameObject enemy = Object.Instantiate(Resources.Load<GameObject>("Prefabs/EnemyRagdoll"));
        enemy.transform.position = new Vector3(0, 0, 0.6f);
        enemy.GetComponent<EnemyController>().OnSpawn();
        float enemyInitialHealth = enemy.GetComponent<Health>().GetCurrentHealth();
        GameObject projectileLauncherTurret = Object.Instantiate(Resources.Load<GameObject>("Prefabs/ProjectileLauncherTurret"));
        projectileLauncherTurret.transform.position = new Vector3(0, 0, 0.1f);
        projectileLauncherTurret.transform.parent = tower.transform;
        projectileLauncherTurret.transform.localScale = new Vector3(1, 1, 1);

        yield return new WaitForSeconds(6f);

        Assert.Less(enemy.GetComponent<Health>().GetCurrentHealth(), enemyInitialHealth);

        Object.Destroy(enemy);
        Object.Destroy(projectileLauncherTurret);
    }
}