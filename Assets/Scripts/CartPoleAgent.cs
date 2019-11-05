using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

/**
 * @brief Cartオブジェクトの動作を観測・報酬を定義するクラス
 */
public class CartPoleAgent : Agent
{
  public GameObject pole_;

  Rigidbody pole_rb_;

  float angle_z_;
  float angle_velocity_z_;
  float cart_x_;

  float kAngleInGoodOrder = 30f;
  float kPositionInGoodOrder = 3f;

  float kPositiveReward = 0.01f;
  float kNegativeReward = -1f;

  float kGainOfMoveAgent = 0.05f;

  int kLeft = 0;
  int kRight = 1;

  /**
   * @brief 初期化
   *
   * 基底クラスの初期化コールとポールオブジェクトの初期化
   */
  public override void InitializeAgent()
  {
    base.InitializeAgent();
    pole_rb_ = pole_.GetComponent<Rigidbody>();
  }

  /**
   * @brief 学習に使う変数の登録
   */
  public override void CollectObservations()
  {
    AddVectorObs(cart_x_);
    AddVectorObs(angle_z_);
    AddVectorObs(angle_velocity_z_);
  }


  public override void AgentAction(float[] vectorAction, string textAction)
  {
    // Cartを移動させる
    MoveAgent(vectorAction[0]);

    // 現在値保持
    cart_x_ = transform.localPosition.x;
    angle_z_ = pole_.transform.localRotation.eulerAngles.z;
    // 角度の正規化
    // 角度は0~360の値を取る
    if (180f < angle_z_ && angle_z_ < 360f)
    {
      angle_z_ -= 360f;
    }
    Console.WriteLine("{0}", angle_z_);
    angle_velocity_z_ = pole_rb_.angularVelocity.z;

    // 報酬の増減
    StateTransition();
  }

  public override void AgentReset()
  {
    transform.localPosition = new Vector3(0f, 0f, 0f);
    pole_.transform.localPosition = new Vector3(0f, 1f, 0f);
    pole_.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    pole_rb_.velocity = new Vector3(0f, 0f, 0f);
    pole_rb_.angularVelocity = new Vector3(0f, 0f, UnityEngine.Random.Range(-1.5f, 1.5f));
  }

  public override void AgentOnDone()
  {
    // Nothing to do
  }

  /**
   * @brief 報酬の増減
   *
   */
  void StateTransition()
  {
    // 角度が小さい間は報酬を与える
    if (Math.Abs(angle_z_) < kAngleInGoodOrder)
    {
      AddReward(kPositiveReward);
    }
    // それ以上は減点
    else
    {
      AddReward(kNegativeReward);
      Done();
    }

    // Cartの位置が原点から離れ過ぎたらだめ
    if (Math.Abs(cart_x_) > kPositionInGoodOrder) {
      AddReward(kNegativeReward);
      Done();
    }
  }

  void MoveAgent(float cart_act)
  {
    int action = Mathf.FloorToInt(cart_act);
    if (action == kLeft)
    {
      transform.Translate(-kGainOfMoveAgent, 0f, 0f);
    }
    else if (action == kRight)
    {
      transform.Translate(kGainOfMoveAgent, 0f, 0f);
    }
  }

  void FixedUpdate()
  {
    // Nothing to do
  }

}
