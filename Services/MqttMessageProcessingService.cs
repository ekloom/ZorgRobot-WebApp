using System;
using SimpleMqtt;

namespace ZorgRobotWebApp.Services;

public class MqttMessageProcessingService : IHostedService
{
  private readonly IUserRepository _userRepository;
  private readonly SimpleMqttClient _mqttClient;

  public MqttMessageProcessingService(IUserRepository userRepository, SimpleMqttClient mqttClient)
  {
    _userRepository = userRepository;
    _mqttClient = mqttClient;

    _mqttClient.OnMessageReceived += HandleMessage;

  }

  public async Task StartAsync(CancellationToken cancellationToken)
  {
    await _mqttClient.SubscribeToTopic("#");

  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }

  private void HandleMessage(object sender, SimpleMqttMessage args)
  {
    // save je data mbv je repo
    // Wllicht wil je ook je data opslaan in een database?
    Console.WriteLine($"Incoming MQTT message on {args.Topic}:{args.Message}");

    User u = new()
    {
      Name = args.Message,
      Age = 12,
      IsActive = true
    };
    _userRepository.SaveUser(u);
  }
}
