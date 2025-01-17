using System;

namespace ZorgRobotWebApp.Services.Mqtt;

public class MqttMessageHandler : IHostedService
{
    private readonly SimpleMqttClient _mqttClient;

    public event EventHandler<string>? OnMessageReceived;

    public MqttMessageHandler(IConfiguration _config)
    {

        var data = _config.GetSection("HiveMQ");

        _mqttClient = new SimpleMqttClient(new SimpleMqttClientConfiguration
        {
            Host = data["Host"],
            Port = Convert.ToInt16(data["Port"]),
            CleanStart = false,
            ClientId = "RobotProject",
            TimeoutInMs = 5_000,
            UserName = data["UserName"],
            Password = data["Password"],

        });

        _mqttClient.OnMessageReceived += HandleMessage;
    }

    public async Task SendMessage(string message)
    {
        string topic = "/Command";

        System.Console.WriteLine($"Command being send to MQTT: Topic = {topic}, Message = {message}");
        await _mqttClient.PublishMessage(message, topic);
    }


    private void HandleMessage(object sender, SimpleMqttMessage? args)
    {
        Console.WriteLine($"Message received: {args.Topic} {args.Message}");
        OnMessageReceived?.Invoke(sender, args.Message);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _mqttClient.SubscribeToTopic("/Alert");
        await _mqttClient.SubscribeToTopic("/Info");
        await _mqttClient.PublishMessage("Webapp is connected!", "ConnectionCheck");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

public enum TopicType
{
    Alert,
    Info,
}
