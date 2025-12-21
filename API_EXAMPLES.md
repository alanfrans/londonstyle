# API Examples

Complete examples for all London Style AI Function App endpoints.

## Base Configuration

```bash
# For local testing
BASE_URL="http://localhost:7071/api"

# For Azure deployment (replace with your URL)
BASE_URL="https://your-app-name.azurewebsites.net/api"
FUNCTION_KEY="your-function-key"
```

---

## 1. Health Check

Verify the service is running.

### cURL

```bash
curl "${BASE_URL}/health"
```

### Python

```python
import requests

response = requests.get(f"{BASE_URL}/health")
print(response.json())
```

### Response

```json
{
  "status": "healthy",
  "service": "londonstyle-ai-functions",
  "version": "1.0.0"
}
```

---

## 2. Simplify Text

Rewrite complex text to 6th-8th grade reading level with active voice.

### Example 1: Passive to Active Voice

#### cURL

```bash
curl -X POST "${BASE_URL}/simplify" \
  -H "Content-Type: application/json" \
  -d '{
    "text": "The implementation of the new policy was conducted by the county commissioners."
  }'
```

#### Python

```python
import requests

response = requests.post(
    f"{BASE_URL}/simplify",
    json={
        "text": "The implementation of the new policy was conducted by the county commissioners."
    }
)
print(response.json())
```

#### Response

```json
{
  "original": "The implementation of the new policy was conducted by the county commissioners.",
  "simplified": "The county commissioners put the new policy into action.",
  "ai_revised": true,
  "grade_level": "6th-8th grade target"
}
```

### Example 2: Complex Sentence

#### cURL

```bash
curl -X POST "${BASE_URL}/simplify" \
  -H "Content-Type: application/json" \
  -d '{
    "text": "In accordance with the provisions set forth in the municipal code, all residents are required to obtain appropriate permits prior to the commencement of any construction activities."
  }'
```

#### Response

```json
{
  "original": "In accordance with the provisions set forth in the municipal code, all residents are required to obtain appropriate permits prior to the commencement of any construction activities.",
  "simplified": "Following city rules, all residents must get permits before starting any construction work.",
  "ai_revised": true,
  "grade_level": "6th-8th grade target"
}
```

---

## 3. Adjust Tone

Change the writing style while maintaining meaning.

### Example 1: Make it Community Friendly

#### cURL

```bash
curl -X POST "${BASE_URL}/adjust-tone" \
  -H "Content-Type: application/json" \
  -d '{
    "text": "Residents are hereby notified that they must procure permits prior to initiating construction activities.",
    "tone": "community_friendly"
  }'
```

#### Python

```python
import requests

response = requests.post(
    f"{BASE_URL}/adjust-tone",
    json={
        "text": "Residents are hereby notified that they must procure permits prior to initiating construction activities.",
        "tone": "community_friendly"
    }
)
print(response.json())
```

#### Response

```json
{
  "original": "Residents are hereby notified that they must procure permits prior to initiating construction activities.",
  "adjusted": "Neighbors, please remember to get your permits before you start building.",
  "tone": "community_friendly",
  "ai_revised": true
}
```

### Example 2: Make it Government Professional

#### cURL

```bash
curl -X POST "${BASE_URL}/adjust-tone" \
  -H "Content-Type: application/json" \
  -d '{
    "text": "Hey everyone, you gotta get a permit before you start building stuff on your property.",
    "tone": "government_professional"
  }'
```

#### Response

```json
{
  "original": "Hey everyone, you gotta get a permit before you start building stuff on your property.",
  "adjusted": "All property owners must obtain the required permits before beginning construction work.",
  "tone": "government_professional",
  "ai_revised": true
}
```

---

## 4. Make Skimmable

Convert long text into easy-to-scan formats.

### Example 1: Bullet Points

#### cURL

```bash
curl -X POST "${BASE_URL}/make-skimmable" \
  -H "Content-Type: application/json" \
  -d '{
    "text": "The county parks department offers various recreational programs throughout the year. These include youth sports leagues in soccer, basketball, and baseball. We also provide summer day camps for children ages 5-12 with activities like arts and crafts, nature exploration, and swimming lessons. Educational workshops are available for adults covering topics such as gardening, cooking, and fitness. All programs are designed to promote community engagement and healthy lifestyles. Registration is available online or in person at any of our five community centers.",
    "format": "bullets"
  }'
```

#### Python

```python
import requests

response = requests.post(
    f"{BASE_URL}/make-skimmable",
    json={
        "text": "The county parks department offers various recreational programs throughout the year. These include youth sports leagues in soccer, basketball, and baseball. We also provide summer day camps for children ages 5-12 with activities like arts and crafts, nature exploration, and swimming lessons. Educational workshops are available for adults covering topics such as gardening, cooking, and fitness. All programs are designed to promote community engagement and healthy lifestyles. Registration is available online or in person at any of our five community centers.",
        "format": "bullets"
    }
)
print(response.json())
```

#### Response

```json
{
  "original": "The county parks department offers various recreational programs throughout the year...",
  "skimmable": "• Youth sports leagues: soccer, basketball, baseball\n• Summer day camps for ages 5-12: arts, crafts, nature, swimming\n• Adult workshops: gardening, cooking, fitness\n• Promotes community engagement and healthy lifestyles\n• Register online or at any of five community centers",
  "format": "bullets",
  "ai_revised": true
}
```

### Example 2: Subheadings

#### cURL

```bash
curl -X POST "${BASE_URL}/make-skimmable" \
  -H "Content-Type: application/json" \
  -d '{
    "text": "The new parking ordinance will take effect on January 1st. All vehicles must display a valid parking permit. Overnight parking is prohibited on city streets between 2 AM and 6 AM unless you have a special permit. Residents can apply for permits online through the city portal. The annual fee is $50 for the first vehicle and $25 for each additional vehicle. Violations will result in a $75 citation. Three violations within a year may result in vehicle towing at the owner'\''s expense.",
    "format": "subheadings"
  }'
```

#### Response

```json
{
  "original": "The new parking ordinance will take effect on January 1st...",
  "skimmable": "## Effective Date\nThe new parking ordinance takes effect January 1st.\n\n## Permit Requirements\nAll vehicles must display a valid parking permit. Overnight parking (2 AM - 6 AM) is prohibited without a special permit.\n\n## How to Get a Permit\nResidents can apply online through the city portal.\n\n## Fees\n$50 for the first vehicle, $25 for each additional vehicle (annual).\n\n## Penalties\n$75 citation per violation. Three violations in one year may result in towing at owner's expense.",
  "format": "subheadings",
  "ai_revised": true
}
```

---

## 5. Detect Jargon

Identify acronyms and jargon with plain language alternatives.

### Example 1: Government Communication

#### cURL

```bash
curl -X POST "${BASE_URL}/detect-jargon" \
  -H "Content-Type: application/json" \
  -d '{
    "text": "The PIO will disseminate the RFP to all stakeholders for procurement of infrastructure. The municipality will utilize synergies to optimize deliverables."
  }'
```

#### Python

```python
import requests

response = requests.post(
    f"{BASE_URL}/detect-jargon",
    json={
        "text": "The PIO will disseminate the RFP to all stakeholders for procurement of infrastructure. The municipality will utilize synergies to optimize deliverables."
    }
)
print(response.json())
```

#### Response

```json
{
  "original": "The PIO will disseminate the RFP to all stakeholders for procurement of infrastructure. The municipality will utilize synergies to optimize deliverables.",
  "jargon_found": [
    {
      "term": "PIO",
      "type": "acronym",
      "definition": "Public Information Officer",
      "plain_language": "communications staff member"
    },
    {
      "term": "RFP",
      "type": "acronym",
      "definition": "Request for Proposal",
      "plain_language": "request for bids"
    },
    {
      "term": "disseminate",
      "type": "jargon",
      "plain_language": "share"
    },
    {
      "term": "procurement",
      "type": "jargon",
      "plain_language": "buying"
    },
    {
      "term": "stakeholders",
      "type": "jargon",
      "plain_language": "interested parties"
    },
    {
      "term": "utilize",
      "type": "jargon",
      "plain_language": "use"
    },
    {
      "term": "synergies",
      "type": "jargon",
      "plain_language": "teamwork"
    },
    {
      "term": "optimize",
      "type": "jargon",
      "plain_language": "improve"
    },
    {
      "term": "deliverables",
      "type": "jargon",
      "plain_language": "results"
    }
  ],
  "ai_revised": false
}
```

### Example 2: Clean Text

#### cURL

```bash
curl -X POST "${BASE_URL}/detect-jargon" \
  -H "Content-Type: application/json" \
  -d '{
    "text": "The parks department will open a new playground next month. Families can enjoy swings, slides, and picnic areas."
  }'
```

#### Response

```json
{
  "original": "The parks department will open a new playground next month. Families can enjoy swings, slides, and picnic areas.",
  "jargon_found": [],
  "ai_revised": false
}
```

---

## Real-World Use Cases

### Use Case 1: Press Release Improvement

**Original**: "The county has commenced implementation of enhanced infrastructure improvements leveraging innovative methodologies to optimize service delivery mechanisms."

```bash
# Step 1: Simplify
curl -X POST "${BASE_URL}/simplify" \
  -H "Content-Type: application/json" \
  -d '{"text": "The county has commenced implementation of enhanced infrastructure improvements leveraging innovative methodologies to optimize service delivery mechanisms."}'

# Step 2: Detect any remaining jargon
curl -X POST "${BASE_URL}/detect-jargon" \
  -H "Content-Type: application/json" \
  -d '{"text": "The county started making improvements to roads and bridges using new methods to provide better services."}'

# Step 3: Make it community friendly
curl -X POST "${BASE_URL}/adjust-tone" \
  -H "Content-Type: application/json" \
  -d '{"text": "The county started making improvements to roads and bridges using new methods to provide better services.", "tone": "community_friendly"}'
```

### Use Case 2: Public Notice

**Original**: Long paragraph about parking changes

```bash
# Make it skimmable with bullets
curl -X POST "${BASE_URL}/make-skimmable" \
  -H "Content-Type: application/json" \
  -d '{
    "text": "Starting next month, parking rules will change. You need a permit. Get it online for $50. Park only in marked spots. No overnight parking without special permit. Questions? Call 555-1234.",
    "format": "bullets"
  }'
```

### Use Case 3: Internal Document Review

```bash
# Check for jargon before sending to public
curl -X POST "${BASE_URL}/detect-jargon" \
  -H "Content-Type: application/json" \
  -d '{
    "text": "Your draft communication text here..."
  }'
```

---

## Integration Examples

### JavaScript/Node.js

```javascript
const axios = require('axios');

async function simplifyText(text) {
  try {
    const response = await axios.post(`${BASE_URL}/simplify`, {
      text: text
    });
    return response.data;
  } catch (error) {
    console.error('Error:', error.message);
  }
}

// Usage
simplifyText("The implementation was conducted by the department.")
  .then(result => console.log(result));
```

### C# / .NET

```csharp
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class LondonStyleClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public LondonStyleClient(string baseUrl)
    {
        _httpClient = new HttpClient();
        _baseUrl = baseUrl;
    }

    public async Task<JsonDocument> SimplifyAsync(string text)
    {
        var payload = new { text = text };
        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync($"{_baseUrl}/simplify", content);
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(result);
    }
}

// Usage
var client = new LondonStyleClient("http://localhost:7071/api");
var result = await client.SimplifyAsync("The implementation was conducted.");
```

### PHP

```php
<?php

function simplifyText($baseUrl, $text) {
    $data = ['text' => $text];
    $options = [
        'http' => [
            'header'  => "Content-type: application/json\r\n",
            'method'  => 'POST',
            'content' => json_encode($data)
        ]
    ];
    
    $context = stream_context_create($options);
    $result = file_get_contents("$baseUrl/simplify", false, $context);
    
    return json_decode($result, true);
}

// Usage
$result = simplifyText("http://localhost:7071/api", "The implementation was conducted.");
print_r($result);
?>
```

---

## Error Handling

### Missing Required Field

```bash
curl -X POST "${BASE_URL}/simplify" \
  -H "Content-Type: application/json" \
  -d '{}'
```

Response (400 Bad Request):
```json
{
  "error": "Missing required field: text"
}
```

### Invalid Tone

```bash
curl -X POST "${BASE_URL}/adjust-tone" \
  -H "Content-Type: application/json" \
  -d '{
    "text": "Sample text",
    "tone": "invalid_tone"
  }'
```

Response (400 Bad Request):
```json
{
  "error": "Invalid tone. Must be \"government_professional\" or \"community_friendly\""
}
```

### Invalid JSON

```bash
curl -X POST "${BASE_URL}/simplify" \
  -H "Content-Type: application/json" \
  -d 'not-valid-json'
```

Response (400 Bad Request):
```json
{
  "error": "Invalid JSON"
}
```

---

## Testing Tips

1. **Use the test script**: `python test_functions.py`
2. **Check response times**: Add `-w "\nTime: %{time_total}s\n"` to cURL
3. **Verify headers**: Add `-i` to cURL to see response headers
4. **Pretty print JSON**: Pipe through `jq`: `curl ... | jq`

## Performance Notes

- Target response time: < 2 seconds
- Longer texts may take more time
- Use appropriate max_tokens for your use case
- Consider caching for repeated requests

---

For more information, see [FUNCTION_APP_README.md](FUNCTION_APP_README.md)
