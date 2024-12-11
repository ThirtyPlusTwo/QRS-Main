/*
QUIET RACING SCRIPT v2.1.2

CREATED AND BUG-TESTED BY THIRTY-TWO
*/

// Script Feature Control Variables
private bool doActiveSteering = true;
private bool doActiveSuspension = true;
private bool doActiveStrength = false;
private bool doAutoERS = false; // Generally best disabled at first
private bool doAntiClang = false; // Generally best disabled at first
private bool doAutoAntiClang = false;
private bool doGyroActions = true;
private bool doAltitudeAirShock = true;
private bool doScriptPanelUpdates = true;
private bool doPitModeManeuver = true;
private bool doExhaustPipeController = false;

// Mode Control
private int defaultSteeringMode = 1;
private int defaultSuspensionMode = 0;
private int defaultStrengthMode = 0;
private int defaultAutoERSMode = 0;
private int defaultScriptScreen = 0;
private int suspensionPitMode = 1;

// Active Steering
private float[][] frontWheelSpeeds = {
    new float[] {25f, 70f, 80f, 95f, 100f},
    new float[] {25f, 70f, 80f, 95f, 100f}
};
private float[][] frontWheelAngles = {
    new float[] {44f, 42f, 40f, 35f, 33f},
    new float[] {44f, 44f, 42f, 36f, 33f}
};
private float[][] rearWheelSpeeds = {
    new float[] {25f, 70f, 100f},
    new float[] {25f, 70f, 85f, 100f}
};
private float[][] rearWheelAngles = {
    new float[] {18f, 3f, 0f},
    new float[] {18f, 3f, 2.2f, 1f}
};
// Active Steering Friction-Based Adjustment
private float[][] frictionChangeFrictions = {
    new float[] {60f, 55f, 50f, 49f, 45f, 40f, 30f},
    new float[] {60f, 55f, 50f, 49f, 45f, 40f, 30f}
};
private float[][] frontWheelAdjustment = {
    new float[] {0f, 0f, -0.10f, -0.25f, -1f, -1.5f, -2f},
    new float[] {0f, 0f, -0.10f, -0.25f, -1f, -1.5f, -2f}
};
private float[][] rearWheelAdjustment = {
    new float[] {0f, -0.5f, -0.7f, -1f, -1f, -1.5f, -2f},
    new float[] {0f, -0.5f, -0.7f, -1f, -1f, -1.5f, -2f}
};

// Active Suspension
private float heightDelta = 0.01f;
private float neutralHeightReturnDelay = 0.5f; // Time in seconds before the height returns to the neutral height
private float[] neutralHeights = { // Make sure not to forget this variable when setting up modes
    0.09f,
    -0.32f
};
private float[][] turningHeightSpeeds = {
    new float[] {0f},
    new float[] {0f}
};
private float[][] frontInsideTurningHeights = {
    new float[] {0.11f},
    new float[] {-0.32f}
};
private float[][] frontOutsideTurningHeights = {
    new float[] {0.11f},
    new float[] {-0.32f}
};
private float[][] rearInsideTurningHeights = {
    new float[] {0.09f},
    new float[] {-0.32f}
};
private float[][] rearOutsideTurningHeights = {
    new float[] {0.11f},
    new float[] {-0.32f}
};

// Active Strength
private float strengthDelta = 0.3f;
private float neutralStrengthReturnDelay = 0.5f; // Time in seconds before the strength returns to the current neutral strength
private float[][] strengthSpeeds = {
    new float[] {0f}
};
private float[][] neutralStrengths = {
    new float[] {16f}
};
private float[][] frontInsideTurningStrengths = {
    new float[] {11f}
};
private float[][] frontOutsideTurningStrengths = {
    new float[] {11f}
};
private float[][] rearInsideTurningStrengths = {
    new float[] {11f}
};
private float[][] rearOutsideTurningStrengths = {
    new float[] {16f}
};

// Better AutoERS
private float[] dutyCycles = { // 0.8f is 80% Duty Cycle
    1f,
    0.8f,
    0.5f,
    0.3f
};
private float[] cycleTimes = { // How long a cycle lasts, 1f is 1s
    1f,
    1f,
    1f,
    1f
};
private float[][] ERSSpeeds = {
    new float[] {0f, 70f, 88f, 94f},
    new float[] {0f, 94f},
    new float[] {0f, 94f},
    new float[] {0f, 94f}
};
private int[][] enableStates = {
    new int[] {1, 0, 1, 0},
    new int[] {1, 0},
    new int[] {1, 0},
    new int[] {1, 0}
};

// Anti-Clang & Auto Anti-Clang
private float antiClangFriction = 10f; // Friction the wheels are set to when Anti-Clang is enabled
private float antiClangFrictionDelta = 5f; // % of friction that the wheels decrease by going to antiClangFriction
private float autoAntiClangActivationSpeed = 20f;
private float[] autoAntiClangSpeeds = { 30f, 80f };
private float[] autoAntiClangAngles = { 7f, 12f }; // Angle in degrees that Anti-Clang checks for to enable

// Gyro Actions
private bool doStabGyro = true;
private float stabGyroEnableDelay = 0.5f; // The duration in seconds that the stability gyro will be reenabled after the user has stopped steering
private float stabGyroMinSpeed = 10f; // The minimum speed of the car for the stability gyro feature to be active
private float stabGyroCheckAltitude = 0f; // Set to 0f for the script to automatically determine this value
private float autoFlippingActivationAngleDegrees = 80f;
private float maxGyroRPM = 30f; // Value in RPM that the Auto-Flipping will set each gyro to respective of their orientation

// Altitude Air Shock
private float airShockCheckAltitude = 0f; // Set to 0f for the script to automatically determine this value
private float airShockCheckDownwardSpeed = 5f; // 5f is 5m/s downward

// Script Panel Updates
private string scriptPanelName = "Script Panel";
private string scriptPanelCustomData = "hudlcd:-0.965:-0.11:0.9:White:1";

// Pit Mode Maneuver
private string pitModeProjectorName = "Pit Projector";
private float minPitSpeed = 25f;

// Exhaust Pipe Controller
private float powerPercentageCheck = 0.446f; // 0.75f is 75% power usage
private float powerSpeedCheck = 60f; // 50f is 50m/s

// PLEASE DO NOT EDIT BELOW HERE UNLESS YOU KNOW WHAT YOU'RE DOING
// Preliminary setup variables
private IMyMotorSuspension[] _suspensions;
private IMyShipController _mainController;
private IMyProgrammableBlock _crsBlock;
private List<IMyGyro> _gyros;
private List<IMyPowerProducer> _powerProducers;
private List<IMyFunctionalBlock> _exhaustPipes;
private IMyTextPanel _scriptPanel;
private IMyProjector _pitProjector;
private string setupErrorMessage = "";
private int numSetupErrors = 0;
private string QRSVersion = "2.1.2";

// Mode Control
private Dictionary<string, FeatureModeControl> _AllModes = new Dictionary<string, FeatureModeControl>();

// Commonly used variables
private float carSpeed;
private float xDirection;
private float zDirection;
private double currentElevation;
private float averageFriction;
private float rotationIndicatorAdded;
private float _delta;
private DateTime currentTime;
private DateTime previousTime;

// Autocalculating stabGyroCheckAltitude and airShockCheckAltitude
private int altiIndex = 0;
private double[] altiArray = new double[21];
private bool altiCalculated = false;

// Active Steering
private bool needsSteeringUpdate = false;
private float calculatedFrontAngle;
private float calculatedRearAngle;
private float calculatedFrontAdjustment;
private float calculatedRearAdjustment;
private float[] frontWheelSlopes;
private float[] frontWheelIntercepts;
private float[] rearWheelSlopes;
private float[] rearWheelIntercepts;
private bool[] doFrictionBasedAdjustment;
private float[] frontWheelAdjustmentSlopes;
private float[] frontWheelAdjustmentIntercepts;
private float[] rearWheelAdjustmentSlopes;
private float[] rearWheelAdjustmentIntercepts;

// Active Suspension
private bool needsSuspensionUpdate = false;
private float suspensionElapsedTime;
private float flHe;
private float frHe;
private float rlHe;
private float rrHe;
private float[] frontInsideTurningHeightSlopes;
private float[] frontInsideTurningHeightIntercepts;
private float[] frontOutsideTurningHeightSlopes;
private float[] frontOutsideTurningHeightIntercepts;
private float[] rearInsideTurningHeightSlopes;
private float[] rearInsideTurningHeightIntercepts;
private float[] rearOutsideTurningHeightSlopes;
private float[] rearOutsideTurningHeightIntercepts;

// Active Strength
private bool needsStrengthUpdate = false;
private float strengthElapsedTime;
private float flStr;
private float frStr;
private float rlStr;
private float rrStr;
private float[] neutralStrengthSlopes;
private float[] neutralStrengthIntercepts;
private float[] frontInsideTurningStrengthSlopes;
private float[] frontInsideTurningStrengthIntercepts;
private float[] frontOutsideTurningStrengthSlopes;
private float[] frontOutsideTurningStrengthIntercepts;
private float[] rearInsideTurningStrengthSlopes;
private float[] rearInsideTurningStrengthIntercepts;
private float[] rearOutsideTurningStrengthSlopes;
private float[] rearOutsideTurningStrengthIntercepts;

// Gyro Actions
private float autoFlippingActivationAngle;
private bool needsGyroUpdate = false;
private bool isDoingFlip = false;
private float gravityDotUp;
private bool gyroFlipOverrideSet = false;
private bool gyroStabOverrideSet = false;
private bool stabGyroCheckState;
private bool gyroCycleCheck = false;
private float gyroElapsedTime;
private float maxGyroRadians;

// Better AutoERS
private bool needsERSUpdate = true;
private bool isERSOn = false;
private bool currentAERSState;
private bool AERSCycleCheck = false;
private float AERSElapsedTime;
private float timeOn;
private float timeOff;

// Anti-Clang & Auto Anti-Clang
private bool isAutoAntiClangActive = false;
private float[] autoAntiClangSlopes;
private float[] autoAntiClangIntercepts;
private float velDotForward;
private float currentActivationAngleDegrees;
private float currentActivationAngle;

// Altitude Air Shock
private float FLHeight;
private float FRHeight;
private float RLHeight;
private float RRHeight;
private float velDotGravity;
private bool isAltiActive = false;

// Pit Mode Maneuver
private bool isDoingManeuver = false;
private string selectedTire = "";
private string[] validTireCompounds = { "ULTRA", "SOFT", "MEDIUM", "HARD", "INT", "WET", "EXTRA", "PRIME" };
private bool suspensionPitModeSet = false;

// Exhaust Pipe Controller
private float inverseMaxPower = 0f; // Inverse because it's calculated once and multiplication is more efficient than division
private float currentPower = 0f;

// Script Panel Updates
private string scriptInformationMessage = "";
private int maxInfoLines = 12;

// Echo States
private string displayMessage = "";
private string[] splitArgument;

public Program()
{
    // Making sure some things are setup correctly
    SetupCurrentStates();
    SetupSingularComponents();
    SetupControlSeat();
    SetupSuspensions();
    SetupEnableDependentVariables();
    SetupProgrammableBlocks();
    CheckSteeringArrayLengths();
    CheckSuspensionArrayLengths();
    CheckStrengthArrayLengths();
    CheckAERSArrayLengths();

    if (HandleErrors(numSetupErrors, setupErrorMessage)) { return; }

    // A large chunk of pre-calculated constants based on the preset values
    CalculateDoFrictionBasedAdjustment();
    SetupDefaultModes();
    CalculateModeConstants();

    Runtime.UpdateFrequency = UpdateFrequency.Update1;
}

private void SetupCurrentStates()
{
    _AllModes.Add("STEER", new FeatureModeControl("Active Steering", doActiveSteering));
    _AllModes.Add("SUS", new FeatureModeControl("Active Suspension", doActiveSuspension));
    _AllModes.Add("STRENGTH", new FeatureModeControl("Active Strength", doActiveStrength));
    _AllModes.Add("AUTOERS", new FeatureModeControl("Better AutoERS", doAutoERS));
    _AllModes.Add("CLANG", new FeatureModeControl("Anti-Clang", doAntiClang));
    _AllModes.Add("AUTOCLANG", new FeatureModeControl("Auto Anti-Clang", doAutoAntiClang));
    _AllModes.Add("GYRO", new FeatureModeControl("Gyro Actions", doGyroActions));
    _AllModes.Add("ALTI", new FeatureModeControl("Altitude Air Shock", doAltitudeAirShock));
    _AllModes.Add("SCRIPT", new FeatureModeControl("Script Panel Updates", doScriptPanelUpdates));
    _AllModes.Add("PIT", new FeatureModeControl("Pit Mode Maneuver", doPitModeManeuver));
    _AllModes.Add("EXHAUST", new FeatureModeControl("Exhaust Pipe Controller", doExhaustPipeController));
}

private void SetupDefaultModes()
{
    _AllModes["STEER"].SetDefaultModes(defaultSteeringMode, frontWheelSpeeds.Length - 1, defaultSteeringMode);
    _AllModes["SUS"].SetDefaultModes(defaultSuspensionMode, neutralHeights.Length - 1, defaultSuspensionMode);
    _AllModes["STRENGTH"].SetDefaultModes(defaultStrengthMode, neutralStrengths.Length - 1, defaultStrengthMode);
    _AllModes["AUTOERS"].SetDefaultModes(defaultAutoERSMode, dutyCycles.Length - 1, defaultAutoERSMode);
    _AllModes["SCRIPT"].SetDefaultModes(defaultScriptScreen, 3, defaultScriptScreen);
}

private void SetupEnableDependentVariables()
{
    SetupGyros();
    SetupPowerProducers();
    SetupExhaustPipes();
}

private void CalculateModeConstants()
{
    CalculateSteeringConstants();
    CalculateSuspensionConstants();
    CalculateStrengthConstants();
    CalculateAERSConstants();
    CalculateAntiClangConstants();
    CalculateAirShockConstants();
    CalculateExhaustConstants();
    CalculateGyroConstants();
}

private bool HandleErrors(int setupErrors, string errorMessage)
{
    if (errorMessage != "")
    {
        Echo("Check the Programmable Block display for a list of errors. Click on \"Edit Text\" to see the whole message.");
        Me.GetSurface(0).WriteText("There are currently " + setupErrors + " setup errors:\n\n" + errorMessage);
        return true;
    }
    return false;
}

private void SetupSingularComponents()
{
    _scriptPanel = GridTerminalSystem.GetBlockWithName(scriptPanelName) as IMyTextPanel;
    if (_scriptPanel == null && _AllModes["SCRIPT"].FeatureState)
    {
        setupErrorMessage += "The \"" + scriptPanelName + "\" is not found. Set doScriptPanelUpdates to false to ignore this.\n\n";
        numSetupErrors++;
    }
    if (_scriptPanel != null && _AllModes["SCRIPT"].FeatureState)
    {
        _scriptPanel.Enabled = true;
        _scriptPanel.Font = "Monospace";
        _scriptPanel.CustomData = scriptPanelCustomData;
    }
    Me.GetSurface(0).Font = "Monospace";

    _pitProjector = GridTerminalSystem.GetBlockWithName(pitModeProjectorName) as IMyProjector;
}

private void SetupSuspensions()
{
    var suspensions = new List<IMyMotorSuspension>();
    GridTerminalSystem.GetBlocksOfType(suspensions, s => s.CubeGrid == Me.CubeGrid);

    if (suspensions.Count != 4)
    {
        setupErrorMessage += "Only supports 4 suspensions.\n\n";
        numSetupErrors++;
    }
    if (setupErrorMessage != "") { return; }

    _suspensions = new IMyMotorSuspension[4];
    for (int i = 0; i < suspensions.Count; i++)
    {
        Vector3D worldDirection = suspensions[i].GetPosition() - _mainController.CenterOfMass;
        Vector3D bodyPosition = Vector3D.TransformNormal(worldDirection, MatrixD.Transpose(_mainController.WorldMatrix));

        if (bodyPosition.X < 0)
        {
            if (bodyPosition.Z < 0)
            {
                _suspensions[0] = (IMyMotorSuspension)suspensions[i];
                _suspensions[0].CustomName = "Wheel Suspension FL";
            }
            else if (bodyPosition.Z > 0)
            {
                _suspensions[2] = (IMyMotorSuspension)suspensions[i];
                _suspensions[2].CustomName = "Wheel Suspension RL";
            }
        }
        else if (bodyPosition.X > 0)
        {
            if (bodyPosition.Z < 0)
            {
                _suspensions[1] = (IMyMotorSuspension)suspensions[i];
                _suspensions[1].CustomName = "Wheel Suspension FR";
            }
            else if (bodyPosition.Z > 0)
            {
                _suspensions[3] = (IMyMotorSuspension)suspensions[i];
                _suspensions[3].CustomName = "Wheel Suspension RR";
            }
        }
    }
}

private void SetupGyros()
{
    if (!_AllModes["GYRO"].FeatureState) { return; }

    var list = new List<IMyTerminalBlock>();

    GridTerminalSystem.GetBlocksOfType<IMyGyro>(list, g => g.CubeGrid == Me.CubeGrid);

    if (list.Count < 1)
    {
        setupErrorMessage += "Gyro Actions requires at least 1 gyroscope. Set doGyroActions to false to ignore this.\n\n";
        numSetupErrors++;
    }
    if (setupErrorMessage != "") { return; }

    _gyros = list.ConvertAll(g => (IMyGyro)g);
    for (int i = 0; i < _gyros.Count; i++)
    {
        _gyros[i].GyroPower = 1f;
        _gyros[i].Enabled = true;
    }
}

private void SetupPowerProducers()
{
    if (!_AllModes["EXHAUST"].FeatureState) { return; }

    var list = new List<IMyTerminalBlock>();

    GridTerminalSystem.GetBlocksOfType<IMyPowerProducer>(list, p => p.CubeGrid == Me.CubeGrid);

    if (list.Count == 0)
    {
        setupErrorMessage += "No Power Producers found. Set doExhaustPipeController to false to ignore this.\n\n";
        numSetupErrors++;
    }
    if (setupErrorMessage != "") { return; }

    _powerProducers = list.ConvertAll(p => (IMyPowerProducer)p);
}

private void SetupExhaustPipes()
{
    if (!_AllModes["EXHAUST"].FeatureState) { return; }

    var listAll = new List<IMyTerminalBlock>();
    var listPipes = new List<IMyTerminalBlock>();

    GridTerminalSystem.GetBlocksOfType<IMyFunctionalBlock>(listAll, p => p.CubeGrid == Me.CubeGrid);

    for (int i = 0; i < listAll.Count; i++)
    {
        if (listAll[i].BlockDefinition.ToString() == "MyObjectBuilder_ExhaustBlock/SmallExhaustPipe")
        {
            listPipes.Add((IMyFunctionalBlock)listAll[i]);
        }
    }
    if (listPipes.Count == 0)
    {
        setupErrorMessage += "No Exhaust Pipe(s) found. Set doExhaustPipeController to false to ignore this.\n\n";
        numSetupErrors++;
    }
    if (setupErrorMessage != "") { return; }

    _exhaustPipes = listPipes.ConvertAll(p => (IMyFunctionalBlock)p);
}

private void SetupProgrammableBlocks()
{
    Me.GetSurface(0).ContentType = ContentType.TEXT_AND_IMAGE;
    Me.CustomName = "PB QRSv" + QRSVersion;

    var testBlock = GridTerminalSystem.GetBlockWithName("PB CRS") as IMyProgrammableBlock;
    if (testBlock != null) {
        _crsBlock = (IMyProgrammableBlock)testBlock;
        _crsBlock.CustomName = "PB CRS";
        return;
    }

    var list = new List<IMyTerminalBlock>();

    GridTerminalSystem.GetBlocksOfType<IMyProgrammableBlock>(list, p => p.CubeGrid == Me.CubeGrid);

    if (list.Count == 1)
    {
        setupErrorMessage += "Missing Programmable Block with CRS.\n\n";
        numSetupErrors++;
    }
    if (list.Count > 2)
    {
        setupErrorMessage += "Only supports 2 Programmable Blocks: 1 with CRS & 1 with QRS.\n\n";
        numSetupErrors++;
    }
    if (setupErrorMessage != "") { return; }

    for (int i = 0; i < list.Count; i++)
    {
        if (list[i] != Me)
        {
            _crsBlock = (IMyProgrammableBlock)list[i];
            _crsBlock.CustomName = "PB CRS";
        }
    }
}

private void SetupControlSeat()
{
    var list = new List<IMyShipController>();

    GridTerminalSystem.GetBlocksOfType<IMyShipController>(list, c => c.CubeGrid == Me.CubeGrid);

    for (int i = 0; i < list.Count; i++)
    {
        if (list[i].BlockDefinition.ToString() == "MyObjectBuilder_Cockpit/PassengerSeatSmallOffset" || list[i].BlockDefinition.ToString() == "MyObjectBuilder_Cockpit/PassengerSeatSmallNew")
        {
            list.Remove(list[i]);
        }
    }

    if (list.Count == 0)
    {
        setupErrorMessage += "No valid IMyShipController Component found on craft.\n\n";
        numSetupErrors++;
    }
    if (setupErrorMessage != "") { return; }

    _mainController = (IMyShipController)list[0];
}

private bool DetermineErrorArrayLengths(int errorMessageNumber, string errorAppendix, string modeAppendix, float comparisonLength, params float[] otherArrayLengths)
{
    float totalOtherArrayLengths = 0f;
    for (int i = 0; i < otherArrayLengths.Length; i++)
    {
        totalOtherArrayLengths += otherArrayLengths[i];
    }
    totalOtherArrayLengths /= otherArrayLengths.Length;

    if (comparisonLength == totalOtherArrayLengths) { return true; }

    switch (errorMessageNumber)
    {
        case 1:
            setupErrorMessage += "There is a mismatch with the number of modes in the variables for " + errorAppendix + ".\n\n";
            break;
        case 2:
            setupErrorMessage += "There is a mismatch with the number of elements in the " + errorAppendix + " arrays for MODE " + modeAppendix + ".\n\n";
            break;
    }

    numSetupErrors++;
    return false;
}

private void CheckSteeringArrayLengths()
{
    if (!_AllModes["STEER"].FeatureState) { return; }

    bool activeSteeringCheck = DetermineErrorArrayLengths(1, "Active Steering", "", frontWheelSpeeds.Length,
        frontWheelSpeeds.Length, rearWheelSpeeds.Length, frontWheelAngles.Length, rearWheelAngles.Length, frictionChangeFrictions.Length, frontWheelAdjustment.Length, rearWheelAdjustment.Length);

    if (!activeSteeringCheck) { return; }
    for (int i = 0; i < frontWheelSpeeds.Length; i++)
    {
        DetermineErrorArrayLengths(2, "Active Steering front wheel speeds/angles", i.ToString(), frontWheelSpeeds[i].Length, frontWheelSpeeds[i].Length, frontWheelAngles[i].Length);
    }

    for (int i = 0; i < rearWheelSpeeds.Length; i++)
    {
        DetermineErrorArrayLengths(2, "Active Steering rear wheel speeds/angles", i.ToString(), rearWheelSpeeds[i].Length, rearWheelSpeeds[i].Length, rearWheelAngles[i].Length);
    }

    for (int i = 0; i < frictionChangeFrictions.Length; i++)
    {
        DetermineErrorArrayLengths(2, "Active Steering friction-based adjustment", i.ToString(), frictionChangeFrictions[i].Length,
            frictionChangeFrictions[i].Length, frontWheelAdjustment[i].Length, rearWheelAdjustment[i].Length);
    }
}

private void CheckSuspensionArrayLengths()
{
    if (!_AllModes["SUS"].FeatureState) { return; }

    bool activeSuspensionCheck = DetermineErrorArrayLengths(1, "Active Suspension", "", neutralHeights.Length,
        neutralHeights.Length, turningHeightSpeeds.Length, frontInsideTurningHeights.Length, frontOutsideTurningHeights.Length, rearInsideTurningHeights.Length, rearOutsideTurningHeights.Length);

    if (!activeSuspensionCheck) { return; }
    for (int i = 0; i < turningHeightSpeeds.Length; i++)
    {
        DetermineErrorArrayLengths(2, "Active Suspension turning", i.ToString(), turningHeightSpeeds[i].Length,
            turningHeightSpeeds[i].Length, frontInsideTurningHeights[i].Length, frontOutsideTurningHeights[i].Length, rearInsideTurningHeights[i].Length, rearOutsideTurningHeights[i].Length);
    }
}

private void CheckStrengthArrayLengths()
{
    if (!_AllModes["STRENGTH"].FeatureState) { return; }

    bool activeStrengthCheck = DetermineErrorArrayLengths(1, "Active Strength", "", strengthSpeeds.Length,
        strengthSpeeds.Length, neutralStrengths.Length, frontInsideTurningStrengths.Length, frontOutsideTurningStrengths.Length, rearInsideTurningStrengths.Length, rearOutsideTurningStrengths.Length);

    if (!activeStrengthCheck) { return; }
    for (int i = 0; i < strengthSpeeds.Length; i++)
    {
        DetermineErrorArrayLengths(2, "Active Strength", i.ToString(), strengthSpeeds[i].Length,
            strengthSpeeds[i].Length, neutralStrengths[i].Length, frontInsideTurningStrengths[i].Length, frontOutsideTurningStrengths[i].Length, rearInsideTurningStrengths[i].Length, rearOutsideTurningStrengths[i].Length);
    }
}

private void CheckAERSArrayLengths()
{
    bool AERSCheck = DetermineErrorArrayLengths(1, "Better AutoERS", "", dutyCycles.Length, cycleTimes.Length, ERSSpeeds.Length, enableStates.Length);

    if (!AERSCheck) { return; }
    for (int i = 0; i < ERSSpeeds.Length; i++)
    {
        DetermineErrorArrayLengths(2, "Better AutoERS ERSSpeeds or enableStates", i.ToString(), ERSSpeeds[i].Length, enableStates[i].Length);
    }
}

private void CalculateSteeringConstants()
{
    if (!_AllModes["STEER"].FeatureState) { return; }

    int nMode = _AllModes["STEER"].CurrentMode;

    // Constant calculation for normal Active Steering
    frontWheelSlopes = CalculateSlopes(frontWheelSpeeds[nMode], frontWheelAngles[nMode]);
    frontWheelIntercepts = CalculateIntercepts(frontWheelSpeeds[nMode], frontWheelAngles[nMode], frontWheelSlopes);
    rearWheelSlopes = CalculateSlopes(rearWheelSpeeds[nMode], rearWheelAngles[nMode]);
    rearWheelIntercepts = CalculateIntercepts(rearWheelSpeeds[nMode], rearWheelAngles[nMode], rearWheelSlopes);

    // Constant calculation for friction-based steering adjustment
    if (doFrictionBasedAdjustment[nMode])
    {
        frontWheelAdjustmentSlopes = CalculateSlopes(frictionChangeFrictions[nMode], frontWheelAdjustment[nMode]);
        frontWheelAdjustmentIntercepts = CalculateIntercepts(frictionChangeFrictions[nMode], frontWheelAdjustment[nMode], frontWheelAdjustmentSlopes);
        rearWheelAdjustmentSlopes = CalculateSlopes(frictionChangeFrictions[nMode], rearWheelAdjustment[nMode]);
        rearWheelAdjustmentIntercepts = CalculateIntercepts(frictionChangeFrictions[nMode], rearWheelAdjustment[nMode], rearWheelAdjustmentSlopes);
    }
}

private void CalculateDoFrictionBasedAdjustment()
{
    doFrictionBasedAdjustment = new bool[frictionChangeFrictions.Length];

    if (!_AllModes["STEER"].FeatureState) { return; }

    for (int i = 0; i < frictionChangeFrictions.Length; i++)
    {
        float totalFrictionChange = 0f;
        for (int j = 0; j < frictionChangeFrictions[i].Length; j++)
        {
            totalFrictionChange += Math.Abs(frictionChangeFrictions[i][j]) + Math.Abs(frontWheelAdjustment[i][j]) + Math.Abs(rearWheelAdjustment[i][j]);
        }
        doFrictionBasedAdjustment[i] = totalFrictionChange != 0f;
    }
}

private void CalculateSuspensionConstants()
{
    if (!_AllModes["SUS"].FeatureState) { return; }

    int nMode = _AllModes["SUS"].CurrentMode;

    frontInsideTurningHeightSlopes = CalculateSlopes(turningHeightSpeeds[nMode], frontInsideTurningHeights[nMode]);
    frontInsideTurningHeightIntercepts = CalculateIntercepts(turningHeightSpeeds[nMode], frontInsideTurningHeights[nMode], frontInsideTurningHeightSlopes);
    frontOutsideTurningHeightSlopes = CalculateSlopes(turningHeightSpeeds[nMode], frontOutsideTurningHeights[nMode]);
    frontOutsideTurningHeightIntercepts = CalculateIntercepts(turningHeightSpeeds[nMode], frontOutsideTurningHeights[nMode], frontOutsideTurningHeightSlopes);
    rearInsideTurningHeightSlopes = CalculateSlopes(turningHeightSpeeds[nMode], rearInsideTurningHeights[nMode]);
    rearInsideTurningHeightIntercepts = CalculateIntercepts(turningHeightSpeeds[nMode], rearInsideTurningHeights[nMode], rearInsideTurningHeightSlopes);
    rearOutsideTurningHeightSlopes = CalculateSlopes(turningHeightSpeeds[nMode], rearOutsideTurningHeights[nMode]);
    rearOutsideTurningHeightIntercepts = CalculateIntercepts(turningHeightSpeeds[nMode], rearOutsideTurningHeights[nMode], rearOutsideTurningHeightSlopes);
}

private void CalculateStrengthConstants()
{
    if (!_AllModes["STRENGTH"].FeatureState) { return; }

    int nMode = _AllModes["STRENGTH"].CurrentMode;

    neutralStrengthSlopes = CalculateSlopes(strengthSpeeds[nMode], neutralStrengths[nMode]);
    neutralStrengthIntercepts = CalculateIntercepts(strengthSpeeds[nMode], neutralStrengths[nMode], neutralStrengthSlopes);
    frontInsideTurningStrengthSlopes = CalculateSlopes(strengthSpeeds[nMode], frontInsideTurningStrengths[nMode]);
    frontInsideTurningStrengthIntercepts = CalculateIntercepts(strengthSpeeds[nMode], frontInsideTurningStrengths[nMode], frontInsideTurningStrengthSlopes);
    frontOutsideTurningStrengthSlopes = CalculateSlopes(strengthSpeeds[nMode], frontOutsideTurningStrengths[nMode]);
    frontOutsideTurningStrengthIntercepts = CalculateIntercepts(strengthSpeeds[nMode], frontOutsideTurningStrengths[nMode], frontOutsideTurningStrengthSlopes);
    rearInsideTurningStrengthSlopes = CalculateSlopes(strengthSpeeds[nMode], rearInsideTurningStrengths[nMode]);
    rearInsideTurningStrengthIntercepts = CalculateIntercepts(strengthSpeeds[nMode], rearInsideTurningStrengths[nMode], rearInsideTurningStrengthSlopes);
    rearOutsideTurningStrengthSlopes = CalculateSlopes(strengthSpeeds[nMode], rearOutsideTurningStrengths[nMode]);
    rearOutsideTurningStrengthIntercepts = CalculateIntercepts(strengthSpeeds[nMode], rearOutsideTurningStrengths[nMode], rearOutsideTurningStrengthSlopes);
}

private void CalculateAERSConstants()
{
    int nMode = _AllModes["AUTOERS"].CurrentMode;

    timeOn = dutyCycles[nMode] * cycleTimes[nMode];
    timeOff = cycleTimes[nMode] - timeOn;
}

private void CalculateAntiClangConstants()
{
    if (!_AllModes["AUTOCLANG"].FeatureState) { return; }

    antiClangFriction = (float)MathHelper.Clamp(antiClangFriction, 0f, 20f);
    autoAntiClangSlopes = CalculateSlopes(autoAntiClangSpeeds, autoAntiClangAngles);
    autoAntiClangIntercepts = CalculateIntercepts(autoAntiClangSpeeds, autoAntiClangAngles, autoAntiClangSlopes);
}

private void CalculateExhaustConstants()
{
    if (!_AllModes["EXHAUST"].FeatureState) { return; }

    for (int i = 0; i < _powerProducers.Count; i++)
    {
        inverseMaxPower += _powerProducers[i].MaxOutput;
    }
    inverseMaxPower = 1 / inverseMaxPower;
}

private void CalculateGyroConstants()
{
    if (!_AllModes["GYRO"].FeatureState) { return; }

    maxGyroRadians = maxGyroRPM * 0.1047198f;
    autoFlippingActivationAngle = (float)Math.Cos(3.14159f - autoFlippingActivationAngleDegrees * 0.01745f);
}

private void CalculateAirShockConstants()
{
    if (!_AllModes["ALTI"].FeatureState) { return; }

    FLHeight = _suspensions[0].Height;
    FRHeight = _suspensions[1].Height;
    RLHeight = _suspensions[2].Height;
    RRHeight = _suspensions[3].Height;
}

public void Main(string argument)
{
    if (HandleErrors(numSetupErrors, setupErrorMessage)) { return; }
    HandleArgument(argument);
    GetCommonlyUsedVariables();
    AutoCalculateAltiValues();
    HandleActiveSteering();
    HandleActiveSuspension();
    HandleActiveStrength();
    HandleAutoERS();
    HandleGyroActions();
    HandleAutoAntiClang();
    HandleAntiClang();
    HandlePitModeManeuver();
    HandleAltitudeAirShock();
    HandleExhaustPipeController();
    HandleEchoState();
    HandleScriptPanel();
}

private void GetCommonlyUsedVariables()
{
    carSpeed = (float)_mainController.GetShipSpeed();
    xDirection = _mainController.MoveIndicator.X;
    zDirection = _mainController.MoveIndicator.Z;
    _mainController.TryGetPlanetElevation(MyPlanetElevation.Surface, out currentElevation);

    rotationIndicatorAdded = 0f;
    if (Math.Abs(_mainController.RotationIndicator.X) > 0.05f || Math.Abs(_mainController.RotationIndicator.Y) > 0.05f)
    {
        rotationIndicatorAdded = (float)(Math.Abs(_mainController.RotationIndicator.X) + Math.Abs(_mainController.RotationIndicator.Y));
    }

    averageFriction = 0f;
    for (int i = 0; i < _suspensions.Length; i++)
    {
        averageFriction += _suspensions[i].Friction;
    }
    averageFriction *= 0.25f;

    currentTime = DateTime.Now;
    _delta = (float)(currentTime - previousTime).TotalSeconds;
    previousTime = currentTime;
}

private void AutoCalculateAltiValues()
{
    if (altiCalculated) { return; }

    if (Math.Abs(stabGyroCheckAltitude) > 0f && Math.Abs(airShockCheckAltitude) > 0f)
    {
        altiArray = new double[0];
        altiCalculated = true;
        return;
    }

    if (altiIndex >= 20 && !altiCalculated)
    {
        Array.Sort(altiArray);
        float altiMedian = (float)altiArray[10];
        altiArray = new double[0];

        stabGyroCheckAltitude = (stabGyroCheckAltitude > 0f) ? stabGyroCheckAltitude : altiMedian + 0.3f;
        airShockCheckAltitude = (airShockCheckAltitude > 0f) ? airShockCheckAltitude : altiMedian + 1.3f;
        altiCalculated = true;
        return;
    }

    if ((int)zDirection >= 0 || (int)xDirection != 0) { return; }

    altiArray[altiIndex] = currentElevation;
    altiIndex++;
    altiCalculated = false;
}

private void HandleArgument(string argument)
{
    if (argument.Trim() == "") { return; }
    splitArgument = argument.ToUpper().Trim().Split(' ');

    for (int i = 0; i < splitArgument.Length; i++)
    {
        string s = splitArgument[i];

        if (s == "") { continue; }

        if (_AllModes.ContainsKey(s) && i != splitArgument.Length - 1)
        {
            if (splitArgument[i + 1] == "-1")
            {
                int decrementMode = (_AllModes[s].CurrentMode - 1 < 0) ? _AllModes[s].MaxMode : _AllModes[s].CurrentMode - 1;
                _AllModes[s].SetMode(decrementMode);
                CalculateModeConstants();
                continue;
            }
            if (splitArgument[i + 1] == "+1")
            {
                int incrementMode = (_AllModes[s].CurrentMode + 1 > _AllModes[s].MaxMode) ? 0 : _AllModes[s].CurrentMode + 1;
                _AllModes[s].SetMode(incrementMode);
                CalculateModeConstants();
                continue;
            }
            if (splitArgument[i + 1] == "P")
            {
                _AllModes[s].SetMode(_AllModes[s].PreviousMode);
                CalculateModeConstants();
                continue;
            }
            if (splitArgument[i + 1].IndexOf(":") < 0)
            {
                try
                {
                    _AllModes[s].SetMode(int.Parse(splitArgument[i + 1]));
                    CalculateModeConstants();
                }
                catch (Exception e)
                {
                    _AllModes[s].FeatureState = !_AllModes[s].FeatureState;
                    SetupEnableDependentVariables();
                }
                continue;
            }

            int assumedMode;
            int nonAssumedMode;

            string[] splitSubArgument = splitArgument[i + 1].Split(':');
            int nMode = _AllModes[s].CurrentMode;

            try { assumedMode = int.Parse(splitSubArgument[0]); }
            catch (Exception e) { assumedMode = 0; }

            try { nonAssumedMode = int.Parse(splitSubArgument[1]); }
            catch (Exception e) { nonAssumedMode = 0; }

            _AllModes[s].SetMode((nMode == assumedMode) ? nonAssumedMode : assumedMode);
            CalculateModeConstants();
            continue;
        }

        if (_AllModes.ContainsKey(s))
        {
            _AllModes[s].FeatureState = !_AllModes[s].FeatureState;
            SetupEnableDependentVariables();
            continue;
        }

        if (TryTireArgument(s, carSpeed) && !suspensionPitModeSet) { continue; }

        if (s == "FLIP") { isDoingFlip = true; continue; }
    }
}

private bool TryTireArgument(string compound, float carSpeed)
{
    if (carSpeed > minPitSpeed) { return false; }
    for (int i = 0; i < validTireCompounds.Length; i++)
    {
        if (compound == validTireCompounds[i])
        {
            selectedTire = compound;
            isDoingManeuver = true;
            return true;
        }
    }
    return false;
}

private void HandleActiveSteering()
{
    int nMode = _AllModes["STEER"].CurrentMode;

    // If Active Steering is off but the steering still needs an update, then the wheels go back to default
    if (!_AllModes["STEER"].FeatureState && needsSteeringUpdate)
    {
        SetFrontRearAngles(frontWheelAngles[nMode][0] * 0.01745f, rearWheelAngles[nMode][0] * 0.01745f);
        needsSteeringUpdate = false;
        return;
    }
    if (!_AllModes["STEER"].FeatureState) { return; }

    needsSteeringUpdate = true;

    calculatedFrontAngle = RangedInterpolationCalculation(carSpeed, frontWheelSpeeds[nMode], frontWheelAngles[nMode], frontWheelSlopes, frontWheelIntercepts, 1);
    calculatedRearAngle = RangedInterpolationCalculation(carSpeed, rearWheelSpeeds[nMode], rearWheelAngles[nMode], rearWheelSlopes, rearWheelIntercepts, 1);

    if (doFrictionBasedAdjustment[nMode])
    {
        calculatedFrontAdjustment = RangedInterpolationCalculation(averageFriction, frictionChangeFrictions[nMode], frontWheelAdjustment[nMode], frontWheelAdjustmentSlopes, frontWheelAdjustmentIntercepts, -1);
        calculatedRearAdjustment = RangedInterpolationCalculation(averageFriction, frictionChangeFrictions[nMode], rearWheelAdjustment[nMode], rearWheelAdjustmentSlopes, rearWheelAdjustmentIntercepts, -1);
        calculatedFrontAngle += calculatedFrontAdjustment;
        calculatedRearAngle += calculatedRearAdjustment;
    }

    calculatedFrontAngle = (float)MathHelper.Clamp(calculatedFrontAngle, 0f, 46f);
    calculatedRearAngle = (float)MathHelper.Clamp(calculatedRearAngle, 0f, 46f);

    SetFrontRearAngles(calculatedFrontAngle * 0.01745f, calculatedRearAngle * 0.01745f);
}

private void HandleActiveSuspension()
{
    int nMode = _AllModes["SUS"].CurrentMode;

    if (!_AllModes["SUS"].FeatureState && needsSuspensionUpdate)
    {
        float h = neutralHeights[nMode];
        SetWheelHeights(h, h, h, h);
        needsSuspensionUpdate = false;
        return;
    }

    // the || check is necessary to prevent Active Suspension from messing with Altitude Air Shock
    if (!_AllModes["SUS"].FeatureState || isAltiActive) { return; }

    // Making sure to reset the suspension height if disabled while cornering
    needsSuspensionUpdate = true;

    if (xDirection == 0)
    {
        if (suspensionElapsedTime < neutralHeightReturnDelay)
        {
            suspensionElapsedTime += _delta;
            return;
        }

        float h = neutralHeights[nMode];

        flHe = GoToValue(_suspensions[0].Height, h, heightDelta);
        frHe = GoToValue(_suspensions[1].Height, h, heightDelta);
        rlHe = GoToValue(_suspensions[2].Height, h, heightDelta);
        rrHe = GoToValue(_suspensions[3].Height, h, heightDelta);
    }
    if (Math.Abs(xDirection) > 0)
    {
        suspensionElapsedTime = 0f;

        float frontInsideHeight = RangedInterpolationCalculation(carSpeed, turningHeightSpeeds[nMode], frontInsideTurningHeights[nMode], frontInsideTurningHeightSlopes, frontInsideTurningHeightIntercepts, 1);
        float frontOutsideHeight = RangedInterpolationCalculation(carSpeed, turningHeightSpeeds[nMode], frontOutsideTurningHeights[nMode], frontOutsideTurningHeightSlopes, frontOutsideTurningHeightIntercepts, 1);
        float rearInsideHeight = RangedInterpolationCalculation(carSpeed, turningHeightSpeeds[nMode], rearInsideTurningHeights[nMode], rearInsideTurningHeightSlopes, rearInsideTurningHeightIntercepts, 1);
        float rearOutsideHeight = RangedInterpolationCalculation(carSpeed, turningHeightSpeeds[nMode], rearOutsideTurningHeights[nMode], rearOutsideTurningHeightSlopes, rearOutsideTurningHeightIntercepts, 1);

        flHe = (xDirection < 0) ? frontInsideHeight : frontOutsideHeight;
        frHe = (xDirection > 0) ? frontInsideHeight : frontOutsideHeight;
        rlHe = (xDirection < 0) ? rearInsideHeight : rearOutsideHeight;
        rrHe = (xDirection > 0) ? rearInsideHeight : rearOutsideHeight;
    }
    SetWheelHeights(flHe, frHe, rlHe, rrHe);
}

private void HandleActiveStrength()
{
    int nMode = _AllModes["STRENGTH"].CurrentMode;

    if (!_AllModes["STRENGTH"].FeatureState && needsStrengthUpdate)
    {
        float s = neutralStrengths[nMode][0];
        SetWheelStrengths(s, s, s, s);
        needsStrengthUpdate = false;
        return;
    }
    if (!_AllModes["STRENGTH"].FeatureState) { return; }

    needsStrengthUpdate = true;

    if (xDirection == 0)
    {
        if (strengthElapsedTime < neutralStrengthReturnDelay)
        {
            strengthElapsedTime += _delta;
            return;
        }
        float s = RangedInterpolationCalculation(carSpeed, strengthSpeeds[nMode], neutralStrengths[nMode], neutralStrengthSlopes, neutralStrengthIntercepts, 1);

        flStr = GoToValue(_suspensions[0].Strength, s, strengthDelta);
        frStr = GoToValue(_suspensions[1].Strength, s, strengthDelta);
        rlStr = GoToValue(_suspensions[2].Strength, s, strengthDelta);
        rrStr = GoToValue(_suspensions[3].Strength, s, strengthDelta);
    }
    if (Math.Abs(xDirection) > 0)
    {
        strengthElapsedTime = 0f;

        float frontInsideStrength = RangedInterpolationCalculation(carSpeed, strengthSpeeds[nMode], frontInsideTurningStrengths[nMode], frontInsideTurningStrengthSlopes, frontInsideTurningStrengthIntercepts, 1);
        float frontOutsideStrength = RangedInterpolationCalculation(carSpeed, strengthSpeeds[nMode], frontOutsideTurningStrengths[nMode], frontOutsideTurningStrengthSlopes, frontOutsideTurningStrengthIntercepts, 1);
        float rearInsideStrength = RangedInterpolationCalculation(carSpeed, strengthSpeeds[nMode], rearInsideTurningStrengths[nMode], rearInsideTurningStrengthSlopes, rearInsideTurningStrengthIntercepts, 1);
        float rearOutsideStrength = RangedInterpolationCalculation(carSpeed, strengthSpeeds[nMode], rearOutsideTurningStrengths[nMode], rearOutsideTurningStrengthSlopes, rearOutsideTurningStrengthIntercepts, 1);

        flStr = (xDirection < 0) ? frontInsideStrength : frontOutsideStrength;
        frStr = (xDirection > 0) ? frontInsideStrength : frontOutsideStrength;
        rlStr = (xDirection < 0) ? rearInsideStrength : rearOutsideStrength;
        rrStr = (xDirection > 0) ? rearInsideStrength : rearOutsideStrength;

        flStr = GoToValue(_suspensions[0].Strength, flStr, strengthDelta);
        frStr = GoToValue(_suspensions[1].Strength, frStr, strengthDelta);
        rlStr = GoToValue(_suspensions[2].Strength, rlStr, strengthDelta);
        rrStr = GoToValue(_suspensions[3].Strength, rrStr, strengthDelta);
    }
    SetWheelStrengths(flStr, frStr, rlStr, rrStr);
}

private void HandleAutoERS()
{
    // This is to preliminarily ensure the ERS is off before enabling and after
    if (!_AllModes["AUTOERS"].FeatureState && needsERSUpdate)
    {
        _crsBlock.TryRun("ERS_OFF");
        needsERSUpdate = false;
        isERSOn = false;
        return;
    }
    if (!_AllModes["AUTOERS"].FeatureState) { AERSElapsedTime = 0f; AERSCycleCheck = false; return; }

    needsERSUpdate = true;
    AERSElapsedTime += _delta;
    if (!AERSCycleCheck)
    {
        AERSElapsedTime = 0f;
        AERSCycleCheck = true;
        return;
    }

    if (AERSElapsedTime > timeOn + timeOff) { AERSCycleCheck = false; return; }
    if (AERSElapsedTime > timeOn)
    {
        if (!isERSOn) { return; }
        _crsBlock.TryRun("ERS_OFF");
        isERSOn = false;
        return;
    }

    int nMode = _AllModes["AUTOERS"].CurrentMode;

    currentAERSState = RangedCurrentAERSState(carSpeed, ERSSpeeds[nMode], enableStates[nMode]);

    if (currentAERSState && !isERSOn)
    {
        _crsBlock.TryRun("ERS_ON");
        isERSOn = true;
    }
    else if (!currentAERSState && isERSOn)
    {
        _crsBlock.TryRun("ERS_OFF");
        isERSOn = false;
    }
}

private void HandleGyroActions()
{
    // This is simply to reset the Stability Gyro if Gyro Actions are disabled
    if (!_AllModes["GYRO"].FeatureState && needsGyroUpdate)
    {
        SetRelativeGyroOverrideValues(_mainController, _gyros, 0f, 0f, 0f);
        for (int i = 0; i < _gyros.Count; i++)
        {
            _gyros[i].GyroOverride = false;
        }
        needsGyroUpdate = false;
        return;
    }
    if (!_AllModes["GYRO"].FeatureState) { return; }

    if (rotationIndicatorAdded > 0.01f && !isDoingFlip)
    {
        for (int i = 0; i < _gyros.Count; i++)
        {
            _gyros[i].GyroOverride = false;
        }
        gyroFlipOverrideSet = false;
        gyroStabOverrideSet = false;
        return;
    }

    // This simply checks to make sure the car's up vector is outside the predefined cone angle
    gravityDotUp = (float)VRageMath.Vector3D.Dot(
        VRageMath.Vector3D.Normalize(_mainController.GetNaturalGravity()),
        VRageMath.Vector3D.Normalize(_mainController.WorldMatrix.Up)
    );
    if (gravityDotUp > autoFlippingActivationAngle || isDoingFlip)
    {
        float gyroZeroAdded = Math.Abs(_gyros[0].Pitch) + Math.Abs(_gyros[0].Yaw) + Math.Abs(_gyros[0].Roll);
        if (gyroFlipOverrideSet && _gyros[0].GyroOverride) { return; }
        float gravityDotRight = (float)VRageMath.Vector3D.Dot(
            VRageMath.Vector3D.Normalize(_mainController.GetNaturalGravity()),
            VRageMath.Vector3D.Normalize(_mainController.WorldMatrix.Right)
        );
        int rollRPMMult = (gravityDotRight <= 0) ? 1 : -1;

        float pitch = 0f;
        float yaw = 0f;
        float roll = maxGyroRadians * rollRPMMult;

        SetRelativeGyroOverrideValues(_mainController, _gyros, pitch, yaw, roll);

        gyroFlipOverrideSet = true;
        gyroStabOverrideSet = false;
        return;
    }
    needsGyroUpdate = true;

    if (!gyroStabOverrideSet)
    {
        SetRelativeGyroOverrideValues(_mainController, _gyros, 0f, 0f, 0f);
        gyroStabOverrideSet = true;
        for (int i = 0; i < _gyros.Count; i++) { _gyros[i].GyroOverride = false; }
        isDoingFlip = false;
        return;
    }
    if (!doStabGyro) { return; }

    if (gyroCycleCheck)
    {
        gyroElapsedTime += _delta;
    }
    if (Math.Abs(xDirection) > 0)
    {
        gyroElapsedTime = 0f;
        gyroCycleCheck = true;
    }
    gyroCycleCheck = gyroElapsedTime < stabGyroEnableDelay;

    if (!altiCalculated) { return; }
    stabGyroCheckState = (currentElevation >= stabGyroCheckAltitude || (int)averageFriction <= antiClangFriction || carSpeed <= stabGyroMinSpeed || gyroCycleCheck);
    for (int i = 0; i < _gyros.Count; i++)
    {
        _gyros[i].GyroOverride = (stabGyroCheckState) ? false : (xDirection == 0f);
    }
}

private void HandleAutoAntiClang()
{
    if (!_AllModes["AUTOCLANG"].FeatureState || _AllModes["CLANG"].FeatureState) { return; }
    _crsBlock.Enabled = true;
    isAutoAntiClangActive = false;

    if (carSpeed < autoAntiClangActivationSpeed) { return; }

    currentActivationAngleDegrees = RangedInterpolationCalculation(carSpeed, autoAntiClangSpeeds, autoAntiClangAngles, autoAntiClangSlopes, autoAntiClangIntercepts, 1);
    currentActivationAngle = (float)Math.Cos(currentActivationAngleDegrees * 0.01745f);

    velDotForward = (float)VRageMath.Vector3D.Dot(
        VRageMath.Vector3D.Normalize(_mainController.GetShipVelocities().LinearVelocity),
        VRageMath.Vector3D.Normalize(_mainController.WorldMatrix.Forward)
    );

    if (velDotForward < 0 || velDotForward > currentActivationAngle) { return; }

    isAutoAntiClangActive = true;
    _crsBlock.Enabled = false;
    for (int i = 0; i < _suspensions.Length; i++)
    {
        _suspensions[i].Friction = GoToValue(_suspensions[i].Friction, antiClangFriction, antiClangFrictionDelta);
    }
}

private void HandleAntiClang()
{
    _crsBlock.Enabled = true;
    if (!_AllModes["CLANG"].FeatureState) { return; }

    _crsBlock.Enabled = false;
    for (int i = 0; i < _suspensions.Length; i++)
    {
        _suspensions[i].Friction = GoToValue(_suspensions[i].Friction, antiClangFriction, antiClangFrictionDelta);
    }
}

private void HandlePitModeManeuver()
{
    if (!_AllModes["PIT"].FeatureState) { return; }

    if (_pitProjector != null)
    {
        _pitProjector.Enabled = _AllModes["SUS"].CurrentMode == suspensionPitMode;
    }

    if (!isDoingManeuver) { return; }

    if (!suspensionPitModeSet)
    {
        _AllModes["SUS"].SetMode(suspensionPitMode);
        CalculateModeConstants();
        suspensionPitModeSet = true;
        return;
    }

    if (carSpeed > 1f)
    {
        _mainController.HandBrake = true;
        _crsBlock.Enabled = false;
        return;
    }

    _crsBlock.Enabled = true;
    _crsBlock.TryRun(selectedTire);
    _AllModes["SUS"].SetMode(_AllModes["SUS"].PreviousMode);
    CalculateModeConstants();
    suspensionPitModeSet = false;
    isDoingManeuver = false;
}

private void HandleAltitudeAirShock()
{
    if (!_AllModes["ALTI"].FeatureState) { return; }

    velDotGravity = -(float)VRageMath.Vector3.Dot(
        VRageMath.Vector3D.Normalize(_mainController.GetNaturalGravity()),
        _mainController.GetShipVelocities().LinearVelocity
    );

    if (-Math.Abs(airShockCheckDownwardSpeed) < velDotGravity) { isAltiActive = false; }
    if (-Math.Abs(airShockCheckDownwardSpeed) >= velDotGravity && (float)currentElevation >= airShockCheckAltitude) { isAltiActive = true; }

    if (!_suspensions[0].AirShockEnabled && isAltiActive && altiCalculated)
    {
        for (int i = 0; i < _suspensions.Length; i++)
        {
            _suspensions[i].AirShockEnabled = true;
        }
        SetWheelHeights(-0.32f, -0.32f, -0.32f, -0.32f);
        return;
    }
    if (isAltiActive) { return; }

    if (_suspensions[0].AirShockEnabled && !isAltiActive)
    {
        for (int i = 0; i < _suspensions.Length; i++)
        {
            _suspensions[i].AirShockEnabled = false;
        }
        SetWheelHeights(FLHeight, FRHeight, RLHeight, RRHeight);
        return;
    }

    FLHeight = _suspensions[0].Height;
    FRHeight = _suspensions[1].Height;
    RLHeight = _suspensions[2].Height;
    RRHeight = _suspensions[3].Height;
}

private void HandleExhaustPipeController()
{
    if (!_AllModes["EXHAUST"].FeatureState) { return; }

    currentPower = 0f;
    for (int i = 0; i < _powerProducers.Count; i++)
    {
        currentPower += _powerProducers[i].CurrentOutput;
    }

    for (int i = 0; i < _exhaustPipes.Count; i++)
    {
        _exhaustPipes[i].Enabled = ((currentPower * inverseMaxPower) > powerPercentageCheck) && (carSpeed > powerSpeedCheck);
    }
}

// Function used for debugging
private void HandleEchoState()
{
    displayMessage = "Running QRSv" + QRSVersion + "\nCreated and bug-tested by Thirty-Two\n\n";

    string echoState;
    foreach (var item in _AllModes)
    {
        echoState = (item.Value.FeatureState) ? "1" : "0";
        displayMessage += echoState + ": " + item.Value.FeatureName + "\n";
    }
    displayMessage += "\nFor more specific information about some of the currently enabled features click \"Edit Text\".";

    Echo(displayMessage);
    Me.GetSurface(0).WriteText(displayMessage);
}

private void HandleScriptPanel()
{
    int currentScreen = _AllModes["SCRIPT"].CurrentMode;

    switch (currentScreen)
    {
        case 0:
            scriptInformationMessage = SuspensionScriptMessage();
            break;
        case 1:
            scriptInformationMessage = AERSAltiScriptMessage();
            break;
        case 2:
            scriptInformationMessage = AntiClangGyroActionsMessage();
            break;
        case 3:
            scriptInformationMessage = AllFeaturesMessage();
            break;
    }

    scriptInformationMessage += (isAutoAntiClangActive) ? "\n----ACLNG ACTIVE----" : "";
    scriptInformationMessage += AddNewLines(maxInfoLines - scriptInformationMessage.Split('\n').Length);
    scriptInformationMessage += CurrentScreenMessage(currentScreen);

    Me.GetSurface(0).WriteText(scriptInformationMessage);
    if (!_AllModes["SCRIPT"].FeatureState) { return; }
    _scriptPanel.WriteText(scriptInformationMessage);
}

private string AddNewLines(int numNewLines)
{
    string returnString = "";
    for (int i = 1; i <= numNewLines; i++)
    {
        returnString += "\n";
    }
    return returnString;
}

private string CurrentScreenMessage(int currentScreen)
{
    string returnMessage = "\n   0  1  2  3\n   ";
    for (int i = 0; i < currentScreen; i++)
    {
        returnMessage += "   ";
    }
    returnMessage += "^";

    return returnMessage;
}

private string SuspensionScriptMessage()
{
    string returnMessage = "";

    returnMessage += (_AllModes["STEER"].FeatureState || _AllModes["SUS"].FeatureState || _AllModes["STRENGTH"].FeatureState) ? "\n" : "";
    returnMessage += (_AllModes["STEER"].FeatureState) ? "STE: " + _AllModes["STEER"].CurrentMode + " " : "";
    returnMessage += (_AllModes["SUS"].FeatureState) ? "SUS: " + _AllModes["SUS"].CurrentMode + " " : "";
    returnMessage += (_AllModes["STRENGTH"].FeatureState) ? "STR: " + _AllModes["STRENGTH"].CurrentMode + " " : "";

    returnMessage += "\n F: " + (_suspensions[0].MaxSteerAngle * 57.296).ToString("00.0") + "°   R: " + (_suspensions[2].MaxSteerAngle * 57.296).ToString("00.0") + "°";
    returnMessage += "\nFL: " + (_suspensions[0].Height * 100f).ToString("00.0") + "cm FR: " + (_suspensions[1].Height * 100f).ToString("00.0") + "cm";
    returnMessage += "\n    " + _suspensions[0].Strength.ToString("00.0") + "%      " + _suspensions[1].Strength.ToString("00.0") + "%";
    returnMessage += "\nRL: " + (_suspensions[2].Height * 100f).ToString("00.0") + "cm RR: " + (_suspensions[3].Height * 100f).ToString("00.0") + "cm";
    returnMessage += "\n    " + _suspensions[2].Strength.ToString("00.0") + "%      " + _suspensions[3].Strength.ToString("00.0") + "%";

    returnMessage += "\n\nFRIC BASED ADJ: ";
    returnMessage += (doFrictionBasedAdjustment[_AllModes["STEER"].CurrentMode] && _AllModes["STEER"].FeatureState) ? "On" : "Off";
    returnMessage += "\nFRIC: " + averageFriction.ToString("00.0") + "%";
    returnMessage += (doFrictionBasedAdjustment[_AllModes["STEER"].CurrentMode] && _AllModes["STEER"].FeatureState) ?
                    "\n F:" + calculatedFrontAdjustment.ToString("00.0").PadLeft(5, ' ') + "°   R:" + calculatedRearAdjustment.ToString("00.0").PadLeft(5, ' ') + "° "
                    : "";

    return returnMessage;
}

private string AERSAltiScriptMessage()
{
    string returnMessage = "";

    returnMessage += "\nAERS: ";
    returnMessage += (_AllModes["AUTOERS"].FeatureState) ? "On " : "Off";
    returnMessage += " MODE: " + _AllModes["AUTOERS"].CurrentMode;
    returnMessage += "\nCUR CYC TIME: " + AERSElapsedTime.ToString("00.0") + "s";
    returnMessage += "\nON " + (dutyCycles[_AllModes["AUTOERS"].CurrentMode] * 100f).ToString("00") + "% OF " + cycleTimes[_AllModes["AUTOERS"].CurrentMode].ToString("00.0") + "s";

    returnMessage += "\n\nALTI: ";
    returnMessage += (_AllModes["ALTI"].FeatureState) ? "On" : "Off";
    returnMessage += (_AllModes["ALTI"].FeatureState || _AllModes["GYRO"].FeatureState) ?
        "\nVERT VELO:" + velDotGravity.ToString("0.000").PadLeft(6, ' ') + "m/s" +
        "\nGYRO ALTI: " + stabGyroCheckAltitude.ToString("0.000000") + "\nAIRS ALTI: " +
        airShockCheckAltitude.ToString("0.000000") + "\nCURR ALTI: " + currentElevation.ToString("0.000000")
        : "";

    return returnMessage;
}

private string AntiClangGyroActionsMessage()
{
    string returnMessage = "";

    returnMessage += "\nCLNG: ";
    returnMessage += (_AllModes["CLANG"].FeatureState) ? "On  " : "Off ";
    returnMessage += "ACLG: ";
    returnMessage += (_AllModes["AUTOCLANG"].FeatureState) ? "On" : "Off";
    returnMessage += (_AllModes["AUTOCLANG"].FeatureState) ? "\nCUR ANG: " + (Math.Acos(velDotForward) * 57.295).ToString("00.0") + "°\nACT ANG: " + currentActivationAngleDegrees.ToString("00.0") + "°" : "";

    returnMessage += "\n\nGYRO: ";
    returnMessage += (_AllModes["GYRO"].FeatureState) ? "On" : "Off";
    returnMessage += (_AllModes["GYRO"].FeatureState) ? "\nCUR ANG: " + (180 - Math.Acos(gravityDotUp) * 57.295).ToString("00.0") + "°\nACT ANG: " + autoFlippingActivationAngleDegrees.ToString("00.0") + "°" : "";
    return returnMessage;
}

private string AllFeaturesMessage()
{
    string returnMessage = "\nQRSv" + QRSVersion + "\n";
    bool newLineToggle = true;
    foreach (var item in _AllModes)
    {
        returnMessage += (newLineToggle) ? "\n" : "";
        returnMessage += item.Key.Substring(0, Math.Min(item.Key.Length, 5)) + ": ";
        returnMessage += (item.Value.FeatureState) ? "On " : "Off ";
        newLineToggle = !newLineToggle;
    }
    return returnMessage;
}

private bool RangedCurrentAERSState(float xAxisValue, float[] xAxisBounds, int[] stepStates)
{
    bool currentState = false;
    for (int i = xAxisBounds.Length - 1; i >= 0; i--)
    {
        if (xAxisValue >= xAxisBounds[i])
        {
            currentState = stepStates[i] == 1;
            break;
        }
    }
    return currentState;
}

private float RangedInterpolationCalculation(float xAxisValue, float[] xAxisBounds, float[] yAxisValues, float[] boundSlopes, float[] boundIntercepts, int comparisonDirection)
{
    float calculatedValue = 0f;
    for (int i = 0; i < xAxisBounds.Length; i++)
    {
        // This is expecting the bounds to be checked as x moves to +x
        if (xAxisValue <= xAxisBounds[i] && comparisonDirection == 1)
        {
            calculatedValue = (i == 0) ? yAxisValues[i] : boundSlopes[i - 1] * xAxisValue + boundIntercepts[i - 1];
            break;
        }
        // This is expecting the bounds to checked as x moves to 0
        if (xAxisValue >= xAxisBounds[i] && comparisonDirection == -1)
        {
            calculatedValue = (i == 0) ? yAxisValues[i] : boundSlopes[i - 1] * xAxisValue + boundIntercepts[i - 1];
            break;
        }
        calculatedValue = yAxisValues[i];
    }
    return calculatedValue;
}

private float[] CalculateSlopes(float[] xArray, float[] yArray)
{
    float[] calculatedArray = new float[xArray.Length - 1];
    for (int i = 0; i < xArray.Length - 1; i++)
    {
        calculatedArray[i] = (yArray[i + 1] - yArray[i]) / (xArray[i + 1] - xArray[i]);
    }
    return calculatedArray;
}

private float[] CalculateIntercepts(float[] xArray, float[] yArray, float[] slopesArray)
{
    float[] calculatedArray = new float[xArray.Length - 1];
    for (int i = 0; i < xArray.Length - 1; i++)
    {
        calculatedArray[i] = yArray[i] - slopesArray[i] * xArray[i];
    }
    return calculatedArray;
}

private float GoToValue(float originalValue, float desiredValue, float delta)
{
    if (originalValue > desiredValue)
    {
        return (originalValue - delta <= desiredValue) ? desiredValue : originalValue - delta;
    }
    else if (originalValue < desiredValue)
    {
        return (originalValue + delta >= desiredValue) ? desiredValue : originalValue + delta;
    }
    return desiredValue;
}

private void SetFrontRearAngles(float frontAngle, float rearAngle)
{
    for (int i = 0; i < _suspensions.Length; i++)
    {
        _suspensions[i].MaxSteerAngle = (i <= 1) ? frontAngle : rearAngle;
    }
}

private void SetWheelHeights(float flHeight, float frHeight, float rlHeight, float rrHeight)
{
    float[] heights = new float[] { flHeight, frHeight, rlHeight, rrHeight };
    for (int i = 0; i < _suspensions.Length; i++)
    {
        if (_suspensions[i].Height == heights[i]) { continue; }
        _suspensions[i].Height = heights[i];
    }
}

private void SetWheelStrengths(float flStrength, float frStrength, float rlStrength, float rrStrength)
{
    float[] strengths = new float[] { flStrength, frStrength, rlStrength, rrStrength };
    for (int i = 0; i < _suspensions.Length; i++)
    {
        if (_suspensions[i].Strength == strengths[i]) { continue; }
        _suspensions[i].Strength = strengths[i];
    }
}

private void SetRelativeGyroOverrideValues(IMyShipController mainController, List<IMyGyro> gyros, float pitch, float yaw, float roll)
{
    // Simply more efficient to check this first, especially since this is a common case
    if (pitch + yaw + roll == 0f)
    {
        for (int i = 0; i < gyros.Count; i++)
        {
            gyros[i].GyroOverride = true;
            gyros[i].Enabled = true;
            gyros[i].GyroPower = 1f;
            gyros[i].Pitch = 0f;
            gyros[i].Yaw = 0f;
            gyros[i].Roll = 0f;
        }
        return;
    }

    Vector3D rotationVec = new Vector3D(-pitch, yaw, roll);
    Vector3D relativeRotationVec = VRageMath.Vector3D.TransformNormal(rotationVec, mainController.WorldMatrix);

    for (int i = 0; i < gyros.Count; i++)
    {
        Vector3D transformedRotationVec = VRageMath.Vector3D.TransformNormal(relativeRotationVec, Matrix.Transpose(gyros[i].WorldMatrix));

        gyros[i].GyroOverride = true;
        gyros[i].Enabled = true;
        gyros[i].GyroPower = 1f;
        gyros[i].Pitch = (float)transformedRotationVec.X;
        gyros[i].Yaw = (float)transformedRotationVec.Y;
        gyros[i].Roll = (float)transformedRotationVec.Z;
    }
}

private class FeatureModeControl
{
    public string FeatureName { get; set; }
    public bool FeatureState { get; set; }
    public int CurrentMode { get; set; }
    public int MaxMode { get; set; }
    public int PreviousMode { get; set; }

    public FeatureModeControl(string featureName, bool featureState)
    {
        FeatureName = featureName;
        FeatureState = featureState;
    }

    public void SetDefaultModes(int currentMode, int maxMode, int previousMode)
    {
        CurrentMode = currentMode;
        MaxMode = maxMode;
        PreviousMode = previousMode;
    }

    public void SetMode(int desiredMode)
    {
        PreviousMode = CurrentMode;
        CurrentMode = (int)MathHelper.Clamp(desiredMode, 0, MaxMode);
    }
}
