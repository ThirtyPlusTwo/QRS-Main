/*
QUIET RACING SCRIPT v1.4

CREATED AND BUG-TESTED BY THIRTY-TWO
*/

// This script preallocates as many variables as possible since reallocation is one of the more computationally expensive
// operations that can be done with SE scripts.

// These are preliminary setup variables
private IMyMotorSuspension[] _suspensions;
private IMyShipController _mainController;
private IMyProgrammableBlock _fsessBlock;
private IMySensorBlock _collisionSensor;
private IMyTimerBlock _antiClangTimer;
private float carSpeed;

// These are the variables for the Active Steering
private bool doActiveSteering = true; // On by default, but you can turn it to false so you can renable every recompile
private bool needsSteeringUpdate = true; // This just resets the steering values if the active steering is off
private int nMode = 0; // Used for the mode which the steering is setup in (default is typically 0)
// These are the lower and higher speeds for the front and rear respectively
private float[] lowerSpeedFront = {77f, 77f, 75f};
private float[] higherSpeedFront = {96f, 96f, 93f};
private float[] lowerSpeedRear = {47f, 47f, 75f};
private float[] higherSpeedRear = {77f, 77f, 93f};
// These are the front and rear angles for the lower and higher speeds respectively
private float[] frontAngleLowerSpeed = {37f, 40f, 39f};
private float[] frontAngleHigherSpeed = {26f, 26f, 37f};
private float[] rearAngleLowerSpeed = {22f, 23f, 12f};
private float[] rearAngleHigherSpeed = {10f, 11f, 0f};
// This is used for the angle adjustment based on the friction percentage of the wheels
private bool[] doFrictionBasedAdjustment = {true, true, false};
private float[] frictionChangePercentage = {0.075f, 0.075f, 0f}; // 0.08f would mean reduce the front and rear angles by 8% when the tires have 33.33% friction
private float frictionChangeSlope;
private float frictionChangeIntercept;
private float frictionChangeCoefficient;
// These are used in linear interpolation calculations
private float calculatedFrontAngle;
private float calculatedRearAngle;
private float frontSlope;
private float rearSlope;
private float frontIntercept;
private float rearIntercept;

// These are the variables for the Clang Control
private bool doAntiClang = false;       // Off by default so that you can enable it while driving
private bool needsClangUpdate = true;
private bool doSensorDetection = false; // In case you want to have something more similar to the Anti-Backmarker System
private float maxFriction = 22f;        // The friction which Clang Control sets the wheels to

// These are the variables for the Better AutoERS
private bool doAutoERS = false;
private bool needsERSUpdate = true;      // This just preliminarily turns off the ERS
private bool isERSOn = false;
private float upperERSSpeedLimit = 88f;  // This is the limit where the Better AutoERS simply turns off ERS
private bool offERSCase;

// These are the variables for the Active Suspension
private bool doActiveSuspension = true;
private float minHeight = 0.13f;
private float maxHeight = 0.07f;
private float heightDelta = 0.005f;
private int oppositeOrSame = 1; // 1 for opposite side of turning, -1 for same side
// opposite tends to be better for cars with lower CoM while same tends to be better for higher CoM
private float leftHeight;
private float rightHeight;
private float xDirection;

// These are the variables used in the Echo States function
private string[] echoStates = new string[4];
private string displayMessage;

// These are the variables used in the HandleArgument function
private string[] splitArgument;

public Program() {
    // Making sure some things are setup correctly
    SetupSingularComponents();
    SetupSuspensions();

    // A large chunk of pre-calculated constants based on the preset values
    CalculateConstants();

    // This simply sets to update the script every tick
    Runtime.UpdateFrequency = UpdateFrequency.Update1;
}

private void CalculateConstants() {
    // Constant calculation for steering angles
    frontSlope = (frontAngleHigherSpeed[nMode] - frontAngleLowerSpeed[nMode]) / (higherSpeedFront[nMode] - lowerSpeedFront[nMode]);
    rearSlope = (rearAngleHigherSpeed[nMode] - rearAngleLowerSpeed[nMode]) / (higherSpeedRear[nMode] - lowerSpeedRear[nMode]);
    frontIntercept = frontAngleLowerSpeed[nMode] - frontSlope * lowerSpeedFront[nMode];
    rearIntercept = rearAngleLowerSpeed[nMode] - rearSlope * lowerSpeedRear[nMode];

    // Constant calculation for friction changing
    frictionChangeSlope = (frictionChangePercentage[nMode]) / (0.66667f);
    frictionChangeIntercept = (1 - frictionChangePercentage[nMode]) - frictionChangeSlope * 0.33333f;
}

private void SetupSingularComponents() {
    _mainController = GridTerminalSystem.GetBlockWithName("Control Seat") as IMyShipController;
    if (_mainController == null) { throw new Exception("The \"Control Seat\" is not named correctly."); }

    _fsessBlock = GridTerminalSystem.GetBlockWithName("Programmable Block (FSESS)") as IMyProgrammableBlock;
    if (_fsessBlock == null) { throw new Exception("The \"Programmable Block (FSESS)\" is not named correctly."); }

    _collisionSensor = GridTerminalSystem.GetBlockWithName("Anti-Clang Sensor") as IMySensorBlock;
    if (_collisionSensor == null && doSensorDetection) { throw new Exception ("The \"Anti-Clang Sensor\" is not named correctly."); }

    _antiClangTimer = GridTerminalSystem.GetBlockWithName("Anti-Clang Timer Block") as IMyTimerBlock;
    if (_antiClangTimer == null && doSensorDetection) { throw new Exception("The \"Anti-Clang Timer Block\" is not named correctly."); }

    Me.GetSurface(0).ContentType = ContentType.TEXT_AND_IMAGE;
}

private void SetupSuspensions() {
    var suspensions = new List<IMyMotorSuspension>();
    GridTerminalSystem.GetBlocksOfType(suspensions, s => s.CubeGrid == Me.CubeGrid);

    if (suspensions.Count != 4) { throw new Exception("Only supports 4 suspensions."); }

    _suspensions = new IMyMotorSuspension[4] {
        suspensions.FirstOrDefault(s => s.CustomName.Contains("FL")),
        suspensions.FirstOrDefault(s => s.CustomName.Contains("FR")),
        suspensions.FirstOrDefault(s => s.CustomName.Contains("RL")),
        suspensions.FirstOrDefault(s => s.CustomName.Contains("RR"))
    };

    foreach (var s in _suspensions) {
        if (s == null) { throw new Exception("Some wheel(s) is(are) not named correctly."); }
    }
}

public void Main(string argument) {
    HandleArgument(argument);
    HandleActiveSteering();
    HandleAntiClang();
    HandleAutoERS();
    HandleActiveSuspension();
    HandleEchoState();
}

private void HandleArgument(string argument) {
    if (argument.Trim() == "") { return; }

    // Use this argument to toggle the Active Steering on or off
    if (argument.ToUpper().Trim() == "STEER") { doActiveSteering = !doActiveSteering; return; }

    // Use this argument to toggle the Clang Control on or off
    if (argument.ToUpper().Trim() == "CLANG") { doAntiClang = !doAntiClang; return; }
    if (argument.ToUpper().Trim() == "TIMER_STOP") { doAntiClang = false; return; }

    // Use this argument to toggle the Better AutoERS on or off
    if (argument.ToUpper().Trim() == "AUTOERS") { doAutoERS = !doAutoERS; return; }

    // Use this argument to toggle the Active Suspension on or off
    if (argument.ToUpper().Trim() == "SUS") { doActiveSuspension = !doActiveSuspension; return; }

    splitArgument = argument.Split(' ');
    if (splitArgument[0].ToUpper().Trim() == "MODE") {
        nMode = int.Parse(splitArgument[1]);

        if (nMode < 0 || nMode > lowerSpeedFront.Length) {
            throw new Exception ("Invalid Mode number. (n < 0 or n > number of modes)");
        }

        CalculateConstants();
        return;
    }
}

private void HandleActiveSteering() {
    // If Active Steering is off but the steering still needs an update, then the wheels go back to default
    if (!doActiveSteering && needsSteeringUpdate) {
        UpdateSteeringAngles(frontAngleLowerSpeed[nMode], rearAngleLowerSpeed[nMode]);
        needsSteeringUpdate = false;
        return;
    }

    // Hopefully, this is self-explanatory
    if (!doActiveSteering) { return; }

    needsSteeringUpdate = true;
    carSpeed = (float)_mainController.GetShipSpeed();

    // Checking the speeds for the front
    if (carSpeed <= lowerSpeedFront[nMode]) {
        calculatedFrontAngle = frontAngleLowerSpeed[nMode];
    } else if (carSpeed >= higherSpeedFront[nMode]) {
        calculatedFrontAngle = frontAngleHigherSpeed[nMode];
    } else if (carSpeed > lowerSpeedFront[nMode] && carSpeed < higherSpeedFront[nMode]) {
        calculatedFrontAngle = frontSlope * carSpeed + frontIntercept;
    }

    // Checking the speeds for the rear
    if (carSpeed <= lowerSpeedRear[nMode]) {
        calculatedRearAngle = rearAngleLowerSpeed[nMode];
    } else if (carSpeed >= higherSpeedRear[nMode]) {
        calculatedRearAngle = rearAngleHigherSpeed[nMode];
    } else if (carSpeed > lowerSpeedRear[nMode] && carSpeed < higherSpeedRear[nMode]) {
        calculatedRearAngle = rearSlope * carSpeed + rearIntercept;
    }

    // Calculating what to change the steering by based on the current friction
    if (doFrictionBasedAdjustment[nMode]) {
        // the 0.01f is needed since SE has the friction as 0<f<100 instead of 0<f<1
        frictionChangeCoefficient = frictionChangeSlope * _suspensions[0].Friction * 0.01f + frictionChangeIntercept;
        calculatedFrontAngle *= frictionChangeCoefficient;
        calculatedRearAngle *= frictionChangeCoefficient;
    }

    // Changing the steering angles respectively
    UpdateSteeringAngles(calculatedFrontAngle, calculatedRearAngle);
}

private void HandleAntiClang() {
    if (!doAntiClang && needsClangUpdate) {
        _fsessBlock.Enabled = true;
        needsClangUpdate = false;
    }

    // Sensor detection
    if (!doAntiClang && doSensorDetection && _collisionSensor.IsActive) { 
        doAntiClang = true;
        _antiClangTimer.StartCountdown(); // The timer block runs the "TIMER_STOP" argument when completed
    }

    // Hopefully, this is self-explanatory
    if (!doAntiClang) { return; }

    needsClangUpdate = true;
    _fsessBlock.Enabled = false;
    // Setting the wheel friction to maxFriction
    foreach(var s in _suspensions) { s.Friction = maxFriction; }
}

private void HandleAutoERS() {
    if (!doAutoERS && needsERSUpdate) {
        _fsessBlock.TryRun("ERS_OFF");
        needsERSUpdate = false;
        isERSOn = false;
        return;
    }

    if (!doAutoERS) { return; }

    needsERSUpdate = true;
    // Only updates if Active Steering is off since it's unnecessary otherwise
    if (!doActiveSteering) { carSpeed = (float)_mainController.GetShipSpeed(); }

    offERSCase = carSpeed >= upperERSSpeedLimit || _mainController.MoveIndicator.Z >= 0 || (int)carSpeed == 0;
    if (offERSCase && isERSOn) {
        _fsessBlock.TryRun("ERS_OFF");
        isERSOn = false;
    } else if (_mainController.MoveIndicator.Z < 0 && carSpeed < upperERSSpeedLimit && !isERSOn) {
        _fsessBlock.TryRun("ERS_ON");
        isERSOn = true;
    }
}

private void HandleActiveSuspension()  {
    if (!doActiveSuspension) { return; }

    // Checking for the user's wanted control direction
    xDirection = _mainController.MoveIndicator.X;

    leftHeight = _suspensions[0].Height;
    rightHeight = _suspensions[1].Height;

    if (xDirection == 0) {
        leftHeight = minHeight;
        rightHeight = minHeight;
    } else {
        // Going up to the maxHeight smoothly depedent on xDirection
        leftHeight = Math.Min(minHeight, Math.Max(maxHeight, leftHeight - (xDirection * heightDelta * oppositeOrSame)));
        rightHeight = Math.Min(minHeight, Math.Max(maxHeight, rightHeight + (xDirection * heightDelta * oppositeOrSame)));
    }

    UpdateSuspensionHeights(leftHeight, rightHeight);
}

// Function used for debugging
private void HandleEchoState() {
    echoStates[0] = doActiveSteering ? "1" : "0";
    echoStates[1] = doAntiClang ? "1" : "0";
    echoStates[2] = doAutoERS ? "1" : "0";
    echoStates[3] = doActiveSuspension ? "1" : "0";

    displayMessage = "Running QRSv1.4\nCreated and bug-tested by Thirty-Two\n\n" + 
                      echoStates[0] + ": Active Steering\n" + 
                      echoStates[1] + ": Anti-Clang\n" + 
                      echoStates[2] + ": Better AutoERS\n" +
                      echoStates[3] + ": Active Suspension\n\n" +
                      "MODE: " + nMode;


    Echo(displayMessage);
    Me.GetSurface(0).WriteText(displayMessage);
}

private void UpdateSteeringAngles(float frontAngle, float rearAngle) {
    for (int i = 0; i < _suspensions.Length; i++) {
        _suspensions[i].MaxSteerAngle = (i <= 1) ? frontAngle * 0.01745f : rearAngle * 0.01745f;
    }
}

private void UpdateSuspensionHeights(float leftHeight, float rightHeight) {
    for (int i = 0; i < _suspensions.Length; i++) {
        _suspensions[i].Height = (i == 0 || i == 2) ? leftHeight : rightHeight;
    }
}