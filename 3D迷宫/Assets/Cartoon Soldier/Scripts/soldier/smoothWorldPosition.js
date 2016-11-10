//Character Controller clibs stairs' steps in a very snappy manner.
//The purpose of this script is to smooth this snappy changes of position.
//This script must be in a game object in between the hierarchy of the character controller and the soldier.

var horizontalSmooth : float = 3.0;
var verticalUpSmooth : float = 10.0;
var verticalDownSmooth : float = 1;//50.0;
private var worldPosition : Vector3;

function LateUpdate () {
	horizontalSmooth = Mathf.Max(horizontalSmooth, 0);
	verticalUpSmooth = Mathf.Max(verticalUpSmooth, 0);
	verticalDownSmooth = Mathf.Max(verticalDownSmooth, 0);
	if (horizontalSmooth == 0){
		worldPosition.x = transform.parent.position.x;
		worldPosition.z = transform.parent.position.z;
	}
	else{
		worldPosition.x = Mathf.Lerp(worldPosition.x, transform.parent.position.x, Time.deltaTime * 50.0 / horizontalSmooth);
		worldPosition.z = Mathf.Lerp(worldPosition.z, transform.parent.position.z, Time.deltaTime * 50.0 / verticalUpSmooth);
	}
	if(worldPosition.y < transform.parent.position.y){
		if(verticalUpSmooth == 0){
			worldPosition.y = transform.parent.position.y;
		}
		else{
			worldPosition.y = Mathf.Lerp(worldPosition.y, transform.parent.position.y, Time.deltaTime * 50.0 / verticalUpSmooth);
		}
	}
	else{
		if(verticalDownSmooth == 0){
			worldPosition.y = transform.parent.position.y;
		}
		else{
			worldPosition.y = Mathf.Lerp(worldPosition.y, transform.parent.position.y, Time.deltaTime * 50.0 / verticalDownSmooth);
		}
	}
	transform.position = worldPosition;
}