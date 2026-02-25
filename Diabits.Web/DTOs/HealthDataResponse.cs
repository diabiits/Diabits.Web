using System.Text.Json.Serialization;

namespace Diabits.Web.DTOs;

// TODO Split out
public record HealthDataResponse(
    IEnumerable<NumericDto> GlucoseLevels,
    IEnumerable<NumericDto> HeartRates,
    IEnumerable<NumericDto> Steps,
    IEnumerable<NumericDto> SleepSessions,
    IEnumerable<WorkoutDto> Workouts,
    IEnumerable<ManualInputDto> Medications,
    IEnumerable<ManualInputDto> Menstruation
);

public abstract class HealthDataPointBaseDto
{
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("type")]
    public HealthDataType HealthDataType { get; set; }

    [JsonPropertyName("dateFrom")]
    public DateTime DateFrom { get; set; }

    [JsonPropertyName("dateTo")]
    public DateTime? DateTo { get; set; }
}

public class NumericDto : HealthDataPointBaseDto
{
    [JsonPropertyName("value")]
    public NumericHealthValueDto HealthValue { get; set; } = null!;
}

public class NumericHealthValueDto
{
    [JsonPropertyName("numericValue")]
    public double NumericValue { get; set; }
}

public class WorkoutDto : HealthDataPointBaseDto
{
    [JsonPropertyName("value")]
    public WorkoutHealthValueDto HealthValue { get; set; } = null!;
}

public class WorkoutHealthValueDto
{
    [JsonPropertyName("workoutActivityType")]
    public string ActivityType { get; set; } = string.Empty;

    [JsonPropertyName("totalEnergyBurned")]
    public double? CaloriesBurned { get; set; }
}

public class ManualInputDto : HealthDataPointBaseDto
{
    // When the DTO represents medication input this object contains name/amount
    [JsonPropertyName("medication")]
    public MedicationValueDto? Medication { get; set; }

    // When the DTO represents a menstruation entry this string holds the flow category (e.g., "LIGHT")
    [JsonPropertyName("flow")]
    public FlowEnum? Flow { get; set; }
}

public class MedicationValueDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    public double Quantity { get; set; }

    [JsonPropertyName("strengthValue")]
    public double StrengthValue { get; set; }

    [JsonPropertyName("strengthUnit")]
    public string StrengthUnit { get; set; } = string.Empty;
}

public enum HealthDataType { BLOOD_GLUCOSE, STEPS, HEART_RATE, SLEEP_SESSION, WORKOUT, MENSTRUATION, MEDICATION }

public enum FlowEnum { SPOTTING, LIGHT, MEDIUM, HEAVY }