﻿using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections;
using System.Collections.Generic;

using UnityEnhancements;

public class CharacterAnimator {

	public static List<SMWAnimation> smwAnimations = new List<SMWAnimation>();

    static void AddAnimatorControllerParameter (AnimatorController controller)
    {
        // Add parameters
        controller.AddParameter(HashID.p_hSpeed, AnimatorControllerParameterType.Float);
        //controller.AddParameter(HashID.p_vSpeed, AnimatorControllerParameterType.Float);
        controller.AddParameter(HashID.p_grounded, AnimatorControllerParameterType.Bool);
        //controller.AddParameter(HashID.p_walled, AnimatorControllerParameterType.Bool);
        controller.AddParameter(HashID.p_changeRunDirectionTrigger, AnimatorControllerParameterType.Trigger);

        controller.AddParameter(HashID.p_hitTrigger, AnimatorControllerParameterType.Trigger);
        //controller.AddParameter(HashID.p_nextStateTrigger, AnimatorControllerParameterType.Trigger);
        //controller.AddParameter(HashID.p_hitted, AnimatorControllerParameterType.Bool);
        controller.AddParameter(HashID.p_gameOver, AnimatorControllerParameterType.Bool);
        controller.AddParameter(HashID.p_headJumped, AnimatorControllerParameterType.Bool);
        controller.AddParameter(HashID.p_dead, AnimatorControllerParameterType.Bool);

        controller.AddParameter(HashID.p_spawn, AnimatorControllerParameterType.Bool);
        controller.AddParameter(HashID.p_Protection, AnimatorControllerParameterType.Bool);
        controller.AddParameter(HashID.p_startProtectionTrigger, AnimatorControllerParameterType.Trigger);
        controller.AddParameter(HashID.p_stopProtectionTrigger, AnimatorControllerParameterType.Trigger);

        controller.AddParameter(HashID.p_rageTrigger, AnimatorControllerParameterType.Trigger);
        controller.AddParameter(HashID.p_rageModusBool, AnimatorControllerParameterType.Bool);
    }

	public static RuntimeAnimatorController Create (SmwCharacterGenerics smwCharacterGenerics, SmwCharacter smwCharacter, Teams teamId)
	{
		string charName = smwCharacter.charName;
		if(charName == "")
		{
			Debug.LogError("smwCharacter hat keinen namen gesetzt!");
			charName = "unnamedChar";
		}
		Debug.Log( "CharacterAnimator" + " Create () " + charName);
		
		string createdCharacterFolderPath = "Animations/Characters/AutoGenerated/"+charName+"_"+teamId ;
		if(!AssetTools.TryCreateFolderWithAssetDatabase (createdCharacterFolderPath, out createdCharacterFolderPath))
		{
			Debug.LogError("Ordner existiert/existerieren nicht und kann/können nicht angelegt werden!\n" + createdCharacterFolderPath);
			return null;
		}
		
		/**
		 * 			AssetDatabase :	All paths are relative to the project folder => paths always = "Assets/..../..." //TODO last folder no SLASH / !!!
		 **/
		
		
		//		string assetCreatedCharacterFolderPath = "Assets/" + createdCharacterFolderPath;
		AnimatorController controller =  AnimatorController.CreateAnimatorControllerAtPath( createdCharacterFolderPath + "/" + charName + "_" + teamId + "_scripted_AnimatorController.controller");

        AddAnimatorControllerParameter(controller);

        #region Layer 0 State Machine

        // Layer 0 State Machine
        AnimatorStateMachine rootStateMachine = controller.layers[0].stateMachine;

        Vector3 anyStatePos = rootStateMachine.anyStatePosition;
        //Vector3 entryStatePos = rootStateMachine.entryPosition;
        Vector3 refStatePos = anyStatePos;
        Vector3 tempStatePos;

        tempStatePos = anyStatePos;
        tempStatePos.x += 200;
        rootStateMachine.anyStatePosition = tempStatePos;

        /*          Add states           */

        tempStatePos.x = refStatePos.x + 200;
        tempStatePos.y = refStatePos.y - 200;
		AnimatorState idleState = rootStateMachine.AddState(HashID.s_Idle, tempStatePos);
        //		idleState.motion = idleAnim;

        tempStatePos.x = refStatePos.x + 200;
        tempStatePos.y = refStatePos.y - 100;
        AnimatorState jumpState = rootStateMachine.AddState(HashID.s_JumpAndFall, tempStatePos);
        //		jumpState.motion = jumpAnim;

        tempStatePos.x = refStatePos.x + 200;
        tempStatePos.y = refStatePos.y - 300;
        AnimatorState runState = rootStateMachine.AddState(HashID.s_Run, tempStatePos);
        //		runState.motion = runAnim;

        tempStatePos.x = refStatePos.x + 200;
        tempStatePos.y = refStatePos.y - 400;
        AnimatorState skidState = rootStateMachine.AddState(HashID.s_ChangeRunDirection, tempStatePos);
        //		skidState.motion = changeRunDirectionAnim;

        tempStatePos.x = refStatePos.x + 600;
        tempStatePos.y = refStatePos.y;
        AnimatorState hittedState = rootStateMachine.AddState(HashID.s_Generic_Hitted, tempStatePos);
        //		hittedState.motion = idleAnim;

        tempStatePos.x = refStatePos.x + 750;
        tempStatePos.y = refStatePos.y - 100;
        AnimatorState headJumpedState = rootStateMachine.AddState(HashID.s_HeadJumped, tempStatePos);
        //		headJumpedState.motion = headJumpedAnim;

        tempStatePos.x = refStatePos.x + 450;
        tempStatePos.y = refStatePos.y - 100;
        AnimatorState gameOverState = rootStateMachine.AddState(HashID.s_GameOver, tempStatePos);
        //		gameOverState.motion = headJumpedAnim;

        tempStatePos.x = refStatePos.x + 600;
        tempStatePos.y = refStatePos.y - 200;
        AnimatorState deadState = rootStateMachine.AddState(HashID.s_Dead, tempStatePos);
        //		deadState.motion = headJumpedAnim;

        tempStatePos.x = refStatePos.x + 750;
        tempStatePos.y = refStatePos.y - 300;
        AnimatorState spawnState = rootStateMachine.AddState(HashID.s_Generic_Spawn, tempStatePos);
		spawnState.AddStateMachineBehaviour(typeof(SpawnStateScript));  //TODO reference zu characterScript direct mitgeben???
                                                                        //		spawnState.AddStateMachineBehaviour(new SpawnStateScript());

        tempStatePos.x = refStatePos.x + 800;
        tempStatePos.y = refStatePos.y - 400;
        AnimatorState spawnDelayState = rootStateMachine.AddState(HashID.s_Generic_SpawnDelay, tempStatePos);
		spawnDelayState.AddStateMachineBehaviour(typeof(SpawnDelayStateScript));    //TODO reference zu characterScript direct mitgeben???
                                                                                    //		spawnState.AddStateMachineBehaviour(new SpawnDelayStateScript());

        //		spawnState.motion = headJumpedAnim;

        //		AnimatorState spawnProtectionState = rootStateMachine.AddState(HashID.s_Generic_SpawnProtection);
        //		spawnProtectionState.motion = headJumpedAnim;

        #endregion

        #region Layer 1 State Machine - Overlay Layer
        /**
		 * Layer 1 - Overlay Layer
		 **/

        // FIX defaultWeight and blendingMode!!!!
        //		// Manual Creating Layer : http://forum.unity3d.com/threads/animatorcontroller-addlayer-doesnt-create-default-animatorstatemachine.307873/#post-2003218
        AnimatorControllerLayer newLayer = new AnimatorControllerLayer();
		newLayer.name = HashID.l_overlay;
		newLayer.stateMachine = new AnimatorStateMachine();
		newLayer.stateMachine.name = newLayer.name;
		newLayer.stateMachine.hideFlags = HideFlags.HideInHierarchy;
		if (AssetDatabase.GetAssetPath(controller) != "")
			AssetDatabase.AddObjectToAsset(newLayer.stateMachine, AssetDatabase.GetAssetPath(controller));
		//Custom
		newLayer.blendingMode = AnimatorLayerBlendingMode.Override;
		newLayer.defaultWeight = 1f;
		controller.AddLayer(newLayer);


		// Another Way:
		//Animator anim;
		//anim.SetLayerWeight (layerIndex, weight)

//		// Auto Creating Layer & StateMachine  AddLayer(string)!!!
//		controller.AddLayer(HashID.l_overlay);
//		controller.layers[1].blendingMode = AnimatorLayerBlendingMode.Additive;			// setzt für die zeit die es aktiv ist die variablen und wenn deaktiviert wird variable auf vorherigen wert gesetzt
//		controller.layers[1].defaultWeight = 1f;

		if(controller.layers[1].stateMachine == null)
		{
			Debug.LogError("stateMachine == null");
//			controller.layers[1].stateMachine = new AnimatorStateMachine();
		}


		// Layer 1 State Machine
//		controller.layers[1].blendingMode = AnimatorLayerBlendingMode.Override;			// setzt für die zeit die es aktiv ist die variablen und wenn deaktiviert wird variable auf vorherigen wert gesetzt
//		controller.layers[1].defaultWeight = 1f;
		AnimatorStateMachine overlayStateMachine = controller.layers[1].stateMachine;

        refStatePos = overlayStateMachine.anyStatePosition;
        tempStatePos.x = refStatePos.x + 200;
        tempStatePos.y = refStatePos.y;
        AnimatorState defaultOverlayState = overlayStateMachine.AddState(HashID.s_l1_Generic_DefaultState, tempStatePos);

        tempStatePos.x = refStatePos.x + 200;
        tempStatePos.y = refStatePos.y - 100;
        AnimatorState invincibleOverlayState = overlayStateMachine.AddState(HashID.s_l1_Generic_Invincible, tempStatePos);

        tempStatePos.x = refStatePos.x + 200;
        tempStatePos.y = refStatePos.y + 100;
        AnimatorState protectionOverlayState = overlayStateMachine.AddState(HashID.s_l1_Generic_Protection, tempStatePos);

        #endregion

        #region Layer 1 State Machine - Overlay Layer Transitions

        AnimatorStateTransition leaveInvincibleEnterDefaultState = invincibleOverlayState.AddTransition(defaultOverlayState);
		//		leaveInvincibleEnterDefaultState.AddCondition(AnimatorConditionMode.If, 0, HashID.p_rageTrigger);
        SetupAnimatorStateTransition(leaveInvincibleEnterDefaultState, 0f, true, 1f, false);

        AnimatorStateTransition leaveProtectionEnterDefaultStateByTime = protectionOverlayState.AddTransition(defaultOverlayState);
        //		leaveInvincibleEnterDefaultState.AddCondition(AnimatorConditionMode.If, 0, HashID.p_rageTrigger);
        SetupAnimatorStateTransition(leaveProtectionEnterDefaultStateByTime, 0f, true, 1f, false);
		
		AnimatorStateTransition leaveProtectionEnterDefaultStateByTrigger = protectionOverlayState.AddTransition(defaultOverlayState);
        SetupAnimatorStateTransition(leaveProtectionEnterDefaultStateByTrigger, 0f, false, 1f, false);
		leaveProtectionEnterDefaultStateByTrigger.AddCondition(AnimatorConditionMode.If, 0, HashID.p_stopProtectionTrigger);				//TODO defaultOverlayState muss kräfte invincible/spawnprotection entfernen??
		
		// Overlay Layer : AnyState Transitions to InvincibleState
		AnimatorStateTransition enterInvincibleOverlayerState = overlayStateMachine.AddAnyStateTransition(invincibleOverlayState);
        SetupAnimatorStateTransition(enterInvincibleOverlayerState, 0f, false, 1f, false);
        enterInvincibleOverlayerState.AddCondition(AnimatorConditionMode.If, 0, HashID.p_rageTrigger);
		
		
		// Overlay Layer : AnyState Transitions to ProtectionState
		AnimatorStateTransition enterProtectionOverlayerState = overlayStateMachine.AddAnyStateTransition(protectionOverlayState);	//TODO rename SpawnProtection to Protection
        SetupAnimatorStateTransition(enterProtectionOverlayerState, 0f, false, 1f, false);
        enterProtectionOverlayerState.AddCondition(AnimatorConditionMode.If, 0, HashID.p_startProtectionTrigger);

        #endregion

        #region Layer 0 State Machine Transitions
        // Layer 0 - Base Layer

        float minHorizontalSpeed = 0.01f;	// setze schwellwert (treshold)
		
		AnimatorStateTransition leaveIdleEnterRunIfGreater = idleState.AddTransition(runState);
        SetupAnimatorStateTransition(leaveIdleEnterRunIfGreater, 0f, false, 1f, false);
		leaveIdleEnterRunIfGreater.AddCondition(AnimatorConditionMode.Greater, minHorizontalSpeed, HashID.p_hSpeed);
		
		AnimatorStateTransition leaveIdleEnterRunIfLower = idleState.AddTransition(runState);
        SetupAnimatorStateTransition(leaveIdleEnterRunIfLower, 0f, false, 1f, false);
        leaveIdleEnterRunIfLower.AddCondition(AnimatorConditionMode.Less, -minHorizontalSpeed, HashID.p_hSpeed);
		
		AnimatorStateTransition leaveRunEnterIdle = runState.AddTransition(idleState);
		leaveRunEnterIdle.AddCondition(AnimatorConditionMode.Greater, -minHorizontalSpeed, HashID.p_hSpeed);
		leaveRunEnterIdle.AddCondition(AnimatorConditionMode.Less, minHorizontalSpeed, HashID.p_hSpeed);
        SetupAnimatorStateTransition(leaveRunEnterIdle, 0f, false, 1f, false);

        AnimatorStateTransition leaveRunEnterJump = runState.AddTransition(jumpState);
        leaveRunEnterJump.AddCondition(AnimatorConditionMode.IfNot, 0, HashID.p_grounded);
        SetupAnimatorStateTransition(leaveRunEnterJump, 0f, false, 1f, false);

        AnimatorStateTransition leaveIdleEnterJump = idleState.AddTransition(jumpState);
        SetupAnimatorStateTransition(leaveIdleEnterJump, 0f, false, 1f, false);
		leaveIdleEnterJump.AddCondition(AnimatorConditionMode.IfNot, 0, HashID.p_grounded);

		
		AnimatorStateTransition leaveJumpEnterIdle = jumpState.AddTransition(idleState);
        SetupAnimatorStateTransition(leaveJumpEnterIdle, 0f, false, 1f, false);
		leaveJumpEnterIdle.AddCondition(AnimatorConditionMode.If, 0, HashID.p_grounded);
		
		AnimatorStateTransition leaveRunEnterSkid = runState.AddTransition(skidState);
        SetupAnimatorStateTransition(leaveRunEnterSkid, 0f, false, 1f, false);
		leaveRunEnterSkid.AddCondition(AnimatorConditionMode.If, 0, HashID.p_changeRunDirectionTrigger);
		
		AnimatorStateTransition leaveSkidEnterRun = skidState.AddTransition(runState);
        SetupAnimatorStateTransition(leaveSkidEnterRun, 0f, true, 1f, false);   //TODO achtung byTime!
        //leaveSkidEnterRun.AddCondition(AnimatorConditionMode.IfNot, 0, HashID.p_changeRunDirectionTrigger);

        // Any State Transistion
        AnimatorStateTransition fallingTransition = rootStateMachine.AddAnyStateTransition(jumpState); //special TODO markt
        SetupAnimatorStateTransition(fallingTransition, 0f, false, 1f, false);
        fallingTransition.AddCondition(AnimatorConditionMode.IfNot, 0, HashID.p_grounded);

        // Any State Transistion
        AnimatorStateTransition hittedTransition = rootStateMachine.AddAnyStateTransition(hittedState);	//special TODO markt
        SetupAnimatorStateTransition(hittedTransition, 0f, false, 1f, false);
		hittedTransition.AddCondition(AnimatorConditionMode.If, 0, HashID.p_hitTrigger);
		
		AnimatorStateTransition leaveHittedEnterHeadJumped = hittedState.AddTransition(headJumpedState);
        SetupAnimatorStateTransition(leaveHittedEnterHeadJumped, 0f, false, 1f, false);
        leaveHittedEnterHeadJumped.AddCondition(AnimatorConditionMode.If, 0, HashID.p_headJumped);		// TODO <-- change to Trigger? p_headJumpedTrigger
		
		AnimatorStateTransition leaveHittedEnterDie = hittedState.AddTransition(deadState);
        SetupAnimatorStateTransition(leaveHittedEnterDie, 0f, false, 1f, false);
        leaveHittedEnterDie.AddCondition(AnimatorConditionMode.If, 0, HashID.p_dead);		// TODO <-- change to name p_dieTrigger
		
		AnimatorStateTransition leaveHittedEnterGameOver = hittedState.AddTransition(gameOverState);
        SetupAnimatorStateTransition(leaveHittedEnterGameOver, 0f, false, 1f, false);
        leaveHittedEnterGameOver.AddCondition(AnimatorConditionMode.If, 0, HashID.p_gameOver);		// TODO <-- change to name p_gameOverTrigger
		
		AnimatorStateTransition leaveHeadJumpedEnterSpawn = headJumpedState.AddTransition(spawnState);
        SetupAnimatorStateTransition(leaveHeadJumpedEnterSpawn, 0f, false, 1f, false);
        leaveHeadJumpedEnterSpawn.AddCondition(AnimatorConditionMode.If, 0, HashID.p_spawn);		// TODO <-- change to name p_spawnTrigger
		
		AnimatorStateTransition leaveDieEnterSpawn = deadState.AddTransition(spawnState);
        SetupAnimatorStateTransition(leaveDieEnterSpawn, 0f, false, 1f, false);
        leaveDieEnterSpawn.AddCondition(AnimatorConditionMode.If, 0, HashID.p_spawn);		// TODO <-- change to name p_spawnTrigger
		
		AnimatorStateTransition leaveSpawnEnterIdle = spawnState.AddTransition(idleState);
        SetupAnimatorStateTransition(leaveSpawnEnterIdle, 0f, true, 1f, false);     //TODO achtung byTime!	//TODO <-- Achtung 	hasExitTime (nach Animation)
        //leaveSpawnEnterIdle.AddCondition(AnimatorConditionMode.If, 0, HashID.p_spawn);		//TODO add condition to enable controlls & enable gravity & & ...

        #endregion

        // init smwAnimations array

        //		int baseLayerStateCount = 0;
        smwAnimations.Clear();		// BUG FIX!
		smwAnimations.Add(new SMWAnimation(charName + "_" + teamId + "_dynamic_Idle",		    24,1,	smwCharacter.GetSprites(teamId, SmwCharacterAnimation.Idle),	    WrapMode.Loop,	idleState));
		smwAnimations.Add(new SMWAnimation(charName + "_" + teamId + "_dynamic_Run",			24,2,   smwCharacter.GetSprites(teamId, SmwCharacterAnimation.Run),	        WrapMode.Loop,	runState));
		smwAnimations.Add(new SMWAnimation(charName + "_" + teamId + "_dynamic_Jump",		    24,1,	smwCharacter.GetSprites(teamId, SmwCharacterAnimation.Jump),	    WrapMode.Loop,	jumpState));
		smwAnimations.Add(new SMWAnimation(charName + "_" + teamId + "_dynamic_Skid",		    24,1,	smwCharacter.GetSprites(teamId, SmwCharacterAnimation.Skid),	    WrapMode.Loop,	skidState));
		smwAnimations.Add(new SMWAnimation(charName + "_" + teamId + "_dynamic_Die",			24,1,	smwCharacter.GetSprites(teamId, SmwCharacterAnimation.Die),	        WrapMode.Loop,	deadState));
		smwAnimations.Add(new SMWAnimation(charName + "_" + teamId + "_dynamic_HeadJumped",	    24,1,	smwCharacter.GetSprites(teamId, SmwCharacterAnimation.HeadJumped),	WrapMode.Loop,	headJumpedState));


		GenerateAnimationClipAssets(smwAnimations, createdCharacterFolderPath, true);
		

		//TODO:: add Generic AnimationClips to characterAnimatorController
		spawnState.motion = smwCharacterGenerics.spawnAnimClip;
		protectionOverlayState.motion = smwCharacterGenerics.protectionAnimClip;
		invincibleOverlayState.motion = smwCharacterGenerics.rageAnimClip;
		//TODO

		//smwCharacter.runtimeAnimatorController = controller;
		smwCharacter.SetRuntimeAnimationController (teamId, controller);
		EditorUtility.SetDirty(smwCharacter);					// save ScriptableObject
		return controller;
	}

	static void GenerateAnimationClipAssets(List<SMWAnimation> smwAnimations, string createdCharacterFolderPath, bool replace)
    {
        for (int i = 0; i < smwAnimations.Count; i++)
        {
            // AnimationClip
            AnimationClip tempAnimClip = new AnimationClip();
#if !UNITY_5
			// Setting it as generic allows you to use the animation clip in the animation controller
			AnimationUtility.SetAnimationType(tempAnimClip, ModelImporterAnimationType.Generic);
#endif
            tempAnimClip.name = smwAnimations[i].name;

            // Frames Per Second //TODO ACHTUNG: 
            tempAnimClip.frameRate = smwAnimations[i].framesPerSecond;

            // LOOP
            //			Debug.Log( "before loopTime = " + AnimationUtility.GetAnimationClipSettings(tempAnimClip).loopTime);
            //			AnimationUtility.GetAnimationClipSettings(tempAnimClip).loopTime = true;
            //			Debug.Log( "after loopTime = " + AnimationUtility.GetAnimationClipSettings(tempAnimClip).loopTime);

            // LOOP WORKS
            //			Debug.Log( "before serializedClip loopTime = " + AnimationUtility.GetAnimationClipSettings(tempAnimClip).loopTime);
            SerializedObject serializedClip = new SerializedObject(tempAnimClip);
            AnimationClipSettings clipSettings = new AnimationClipSettings(serializedClip.FindProperty("m_AnimationClipSettings"));
            clipSettings.loopTime = true;
            serializedClip.ApplyModifiedProperties();
            //			Debug.Log( "after serializedClip loopTime = " + AnimationUtility.GetAnimationClipSettings(tempAnimClip).loopTime);


            // Wrap Mode (Loop, Once, PingPong....)
            tempAnimClip.wrapMode = smwAnimations[i].wrapMode;
            //tempAnimClip.

            // Setup EditorCurveBinding of Animation Clip
            //			CreateAnimationClip(tempAnimClip, smwAnimations[i].sprites, smwAnimations[i].keyFrames, 1.0f/smwAnimations[i].framesPerSecond);
            CreateAnimationClip(tempAnimClip, smwAnimations[i]);

            // Add AnimationClip to State of StateMachine
            smwAnimations[i].animState.motion = tempAnimClip;

            // In order to insure better interpolation of quaternions, call this function after you are finished setting animation curves.
            tempAnimClip.EnsureQuaternionContinuity();

            if (AssetDatabase.Contains(tempAnimClip))
            {
                Debug.LogError(tempAnimClip.name + " in AssetDatabase bereits enthalten, darf nicht erscheinen");
            }
            else
            {
                //					Debug.Log(tempAnimClip.name + " nicht in AssetDatabase vorhanden, wird jetzt gespeichert.");

                // asset anlegen
                //					Debug.Log("Versuche " + tempAnimClip.name + " in Ordner " + "Assets/"+createdCharacterFolderPath + " zu speicheren");

				string animClipAssetPath = createdCharacterFolderPath + "/" + tempAnimClip.name + ".asset";
				if (!replace)
					animClipAssetPath = AssetTools.GenerateUniqueAssetPath (animClipAssetPath);
				
				AssetDatabase.CreateAsset(tempAnimClip, animClipAssetPath);
            }
        }
        AssetDatabase.SaveAssets();
    }

    static void SetupAnimatorStateTransition (AnimatorStateTransition transition, float duration, bool hasExitTime, float exitTime, bool canTransitionToSelf)
    {
        transition.duration = duration;
        transition.hasExitTime = hasExitTime;
        transition.exitTime = exitTime;
        transition.canTransitionToSelf = canTransitionToSelf;
    }


    static void CreateAnimationClip(AnimationClip animClip, SMWAnimation animSettings )
	{
		Sprite[] sprites = animSettings.sprites;
		int animationLength = animSettings.keyFrames;
		float singleFrameTime = 1.0f / animSettings.framesPerSecond;
//		Debug.Log("Create Animation: " + animClip.name + " " + " Spritearray = " + sprites.Length + ", Animation length = " + animationLength + ", single frame time = " + singleFrameTime );
		// First you need to create e Editor Curve Binding
		EditorCurveBinding curveBinding = new EditorCurveBinding();
		
		// I want to change the sprites of the sprite renderer, so I put the typeof(SpriteRenderer) as the binding type.
		curveBinding.type = typeof(SpriteRenderer);
		// Regular path to the gameobject that will be changed (empty string means root)
		curveBinding.path = "";
		// This is the property name to change the sprite of a sprite renderer
		curveBinding.propertyName = "m_Sprite";

//		curveBinding.propertyName = "

		// An array to hold the object keyframes
		ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[animationLength];
		
		for (int i = 0; i < sprites.Length; i++)
		{
			keyFrames[i] = new ObjectReferenceKeyframe();
			// set the time
			keyFrames[i].time = i*singleFrameTime;			// TODO important
			// set reference for the sprite you want
			keyFrames[i].value = sprites[i];
			
		}
		
		AnimationUtility.SetObjectReferenceCurve(animClip, curveBinding, keyFrames);
	}
}
