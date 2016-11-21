@script RequireComponent(soldierMovement);
@script RequireComponent(crouchController);

var gunLocaion : String = "soldierCharacter/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Spine2/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Bip01 R Forearm/Bip01 R Hand/gun";
var aimSpeed : float = 5.0;

private var firing : boolean = false;
private var gun : Transform;
private var crosshair : Transform;
private var accuracyLoss : float;
private var accuracyLossTarget : float;
private var shootingAimLoss : float;
private var vibratingAimLoss : float; //shootingAimLoss with firing vibration.
private var isSprinting : boolean;
//External scripts.
private var gunScript : gun;
private var crosshairScript : crosshair;
private var soldierMovementScript : soldierMovement;
private var crouchControllerScript : crouchController;
private var healthScript : health;

function Start(){
	gun = transform.Find(gunLocaion);
	crosshair = transform.Find("crosshair");
	gunScript = gun.GetComponent("gun");
	//External scripts.
	crosshairScript = crosshair.GetComponent("crosshair");
	soldierMovementScript = GetComponent("soldierMovement");
	crouchControllerScript = GetComponent("crouchController");
	healthScript = GetComponent("health");
}

function Update () {
	var health : float = 100;
	if(healthScript != null){
		health = healthScript.GetHealth();
	}
	//Input.
	var isGrounded : boolean = soldierMovementScript.isGrounded;
	if (Input.GetMouseButton(0) && !isSprinting && isGrounded && health > 0){
		firing  = true;
		gunScript.firing = firing;
	}
	else{
		firing = false;
		gunScript.firing = firing;
	}
	//Accuracy.
	var aimCrouchMultiplier : float	= 1 + crouchControllerScript.globalCrouchBlend *10;
	var turnSpeed : float = soldierMovementScript.turnSpeed;
	var forwardSpeed : float = soldierMovementScript.forwardSpeed;
	var strafeSpeed : float = soldierMovementScript.strafeSpeed;
	accuracyLossTarget = 1.0;
	if (forwardSpeed > soldierMovementScript.forwardSpeedMultiplier*1.2){
		isSprinting = true;
		accuracyLossTarget += 1.0;
	}
	else{
		isSprinting = false;
	}
	
	if(gunScript.firing){
		shootingAimLoss = Mathf.Lerp(shootingAimLoss, 2.0, Time.deltaTime* 2.0);
		crosshairScript.yOffset += Random.Range(0,0.5)*Time.deltaTime;
		crosshairScript.xOffset += Random.Range(-0.05,shootingAimLoss*0.1)*Time.deltaTime;
	}	
	else{
		shootingAimLoss = Mathf.Lerp(shootingAimLoss, 0, Time.deltaTime * 20.0);
	}
	vibratingAimLoss = shootingAimLoss + Mathf.Sin(Time.time*80)*shootingAimLoss*0.5;
	accuracyLossTarget += vibratingAimLoss;
	accuracyLossTarget += Mathf.Pow(Mathf.Abs(forwardSpeed * 2.0 + strafeSpeed * 2.0),0.1);
	accuracyLossTarget += Mathf.Pow(Mathf.Pow(Mathf.Abs(turnSpeed), 2.3) / Mathf.Pow(10,4), 0.35);
	accuracyLossTarget += (1- crouchControllerScript.globalCrouchBlend) * 0.5;
	if(accuracyLoss > accuracyLossTarget){
		accuracyLoss = Mathf.Lerp(accuracyLoss, accuracyLossTarget, Time.deltaTime * aimSpeed * aimCrouchMultiplier * 0.5);//Gain aim.
	}
	else{
		accuracyLoss = Mathf.Lerp(accuracyLoss, accuracyLossTarget, Time.deltaTime * aimSpeed);//Lose aim.
	}
	crosshairScript.accuracyLoss = accuracyLoss;
	accuracyLoss = Mathf.Max(accuracyLoss, 1.0);
	var accuracy : float = 1 / accuracyLoss;
	gunScript. accuracy = accuracy;
}

function isFiring() : boolean{
	return firing;
}