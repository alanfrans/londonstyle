import azure.functions as func
import logging
import json
import os
from openai import AzureOpenAI, APIError, APITimeoutError, RateLimitError
from typing import Dict, Any

app = func.FunctionApp(http_auth_level=func.AuthLevel.FUNCTION)

# Initialize Azure OpenAI client
def get_openai_client():
    """Initialize and return Azure OpenAI client."""
    return AzureOpenAI(
        api_key=os.environ.get("AZURE_OPENAI_API_KEY"),
        api_version=os.environ.get("AZURE_OPENAI_API_VERSION", "2024-02-15-preview"),
        azure_endpoint=os.environ.get("AZURE_OPENAI_ENDPOINT")
    )

def create_response(status_code: int, body: Dict[str, Any]) -> func.HttpResponse:
    """
    Create HTTP response with privacy headers.
    Implements zero-retention policy through response headers.
    """
    response = func.HttpResponse(
        json.dumps(body),
        status_code=status_code,
        mimetype="application/json"
    )
    # Add privacy headers
    response.headers['X-Content-Type-Options'] = 'nosniff'
    response.headers['X-Frame-Options'] = 'SAMEORIGIN'
    response.headers['X-Data-Retention'] = 'ephemeral'
    response.headers['Cache-Control'] = 'no-store, no-cache, must-revalidate, private'
    return response

@app.route(route="simplify", methods=["POST"])
def simplify(req: func.HttpRequest) -> func.HttpResponse:
    """
    AI-Powered Rewriting (The "Simplify" Engine)
    
    Simplifies hard-to-read sentences to 6th-8th grade reading level.
    Prioritizes active voice over passive voice.
    
    Request Body:
    {
        "text": "The sentence to simplify"
    }
    
    Response:
    {
        "original": "Original text",
        "simplified": "Simplified text",
        "ai_revised": true,
        "grade_level": "7th grade"
    }
    """
    logging.info('Simplify function processed a request.')
    
    try:
        req_body = req.get_json()
        text = req_body.get('text')
        
        if not text:
            return create_response(400, {
                'error': 'Missing required field: text'
            })
        
        client = get_openai_client()
        deployment_name = os.environ.get("AZURE_OPENAI_DEPLOYMENT_NAME")
        
        # Prompt engineering for simplification with active voice emphasis
        system_prompt = """You are a Plain Language expert helping government employees write clearly.
Your task is to simplify sentences to a 6th-8th grade reading level while:
1. Maintaining the original factual meaning
2. Converting passive voice to active voice
3. Using shorter, simpler words
4. Breaking long sentences into shorter ones if needed

Respond ONLY with the simplified text, no explanations."""

        user_prompt = f"Simplify this sentence for plain language (6th-8th grade level): {text}"
        
        response = client.chat.completions.create(
            model=deployment_name,
            messages=[
                {"role": "system", "content": system_prompt},
                {"role": "user", "content": user_prompt}
            ],
            temperature=0.3,
            max_tokens=500
        )
        
        simplified_text = response.choices[0].message.content.strip()
        
        return create_response(200, {
            'original': text,
            'simplified': simplified_text,
            'ai_revised': True,
            'grade_level': '6th-8th grade target'
        })
        
    except ValueError:
        return create_response(400, {'error': 'Invalid JSON'})
    except APITimeoutError:
        logging.error("Azure OpenAI timeout in simplify function")
        return create_response(504, {'error': 'AI service timeout. Please try again.'})
    except RateLimitError:
        logging.error("Azure OpenAI rate limit exceeded in simplify function")
        return create_response(429, {'error': 'Rate limit exceeded. Please try again later.'})
    except APIError as e:
        logging.error(f"Azure OpenAI API error in simplify function: {type(e).__name__}")
        return create_response(502, {'error': 'AI service error. Please try again.'})
    except Exception as e:
        logging.error(f"Unexpected error in simplify function: {type(e).__name__}")
        return create_response(500, {'error': 'Internal server error'})

@app.route(route="adjust-tone", methods=["POST"])
def adjust_tone(req: func.HttpRequest) -> func.HttpResponse:
    """
    Tone & Tone Adjustment
    
    Adjusts text to "Government Professional" or "Community Friendly" tone.
    
    Request Body:
    {
        "text": "The text to adjust",
        "tone": "government_professional" or "community_friendly"
    }
    
    Response:
    {
        "original": "Original text",
        "adjusted": "Tone-adjusted text",
        "tone": "government_professional",
        "ai_revised": true
    }
    """
    logging.info('Adjust tone function processed a request.')
    
    try:
        req_body = req.get_json()
        text = req_body.get('text')
        tone = req_body.get('tone', 'government_professional')
        
        if not text:
            return create_response(400, {
                'error': 'Missing required field: text'
            })
        
        if tone not in ['government_professional', 'community_friendly']:
            return create_response(400, {
                'error': 'Invalid tone. Must be "government_professional" or "community_friendly"'
            })
        
        client = get_openai_client()
        deployment_name = os.environ.get("AZURE_OPENAI_DEPLOYMENT_NAME")
        
        # Tone-specific prompts
        if tone == 'government_professional':
            system_prompt = """You are a government communications expert.
Adjust the text to a professional government tone while keeping it plain and clear.
- Use formal but accessible language
- Maintain official authority
- Keep vocabulary simple but professional
- Use terms like "residents," "services," "department"

Respond ONLY with the adjusted text, no explanations."""
        else:  # community_friendly
            system_prompt = """You are a community engagement expert.
Adjust the text to a warm, community-friendly tone while keeping it clear.
- Use conversational, friendly language
- Replace formal terms with everyday words (e.g., "procure" → "get", "residents" → "neighbors")
- Keep the tone welcoming and accessible
- Maintain accuracy and respect

Respond ONLY with the adjusted text, no explanations."""
        
        user_prompt = f"Adjust this text for {tone.replace('_', ' ')} tone: {text}"
        
        response = client.chat.completions.create(
            model=deployment_name,
            messages=[
                {"role": "system", "content": system_prompt},
                {"role": "user", "content": user_prompt}
            ],
            temperature=0.5,
            max_tokens=500
        )
        
        adjusted_text = response.choices[0].message.content.strip()
        
        return create_response(200, {
            'original': text,
            'adjusted': adjusted_text,
            'tone': tone,
            'ai_revised': True
        })
        
    except ValueError:
        return create_response(400, {'error': 'Invalid JSON'})
    except APITimeoutError:
        logging.error("Azure OpenAI timeout in adjust_tone function")
        return create_response(504, {'error': 'AI service timeout. Please try again.'})
    except RateLimitError:
        logging.error("Azure OpenAI rate limit exceeded in adjust_tone function")
        return create_response(429, {'error': 'Rate limit exceeded. Please try again later.'})
    except APIError as e:
        logging.error(f"Azure OpenAI API error in adjust_tone function: {type(e).__name__}")
        return create_response(502, {'error': 'AI service error. Please try again.'})
    except Exception as e:
        logging.error(f"Unexpected error in adjust_tone function: {type(e).__name__}")
        return create_response(500, {'error': 'Internal server error'})

@app.route(route="make-skimmable", methods=["POST"])
def make_skimmable(req: func.HttpRequest) -> func.HttpResponse:
    """
    Automated Summarization/Skimmability
    
    Converts long paragraphs into bulleted lists or adds descriptive subheadings.
    
    Request Body:
    {
        "text": "The text to make skimmable",
        "format": "bullets" or "subheadings" (optional, defaults to "bullets")
    }
    
    Response:
    {
        "original": "Original text",
        "skimmable": "Formatted text with bullets or subheadings",
        "format": "bullets",
        "ai_revised": true
    }
    """
    logging.info('Make skimmable function processed a request.')
    
    try:
        req_body = req.get_json()
        text = req_body.get('text')
        format_type = req_body.get('format', 'bullets')
        
        if not text:
            return create_response(400, {
                'error': 'Missing required field: text'
            })
        
        if format_type not in ['bullets', 'subheadings']:
            return create_response(400, {
                'error': 'Invalid format. Must be "bullets" or "subheadings"'
            })
        
        client = get_openai_client()
        deployment_name = os.environ.get("AZURE_OPENAI_DEPLOYMENT_NAME")
        
        # Format-specific prompts
        if format_type == 'bullets':
            system_prompt = """You are a document formatting expert for government communications.
Convert the given text into a clear, skimmable bulleted list.
- Extract key points and present them as bullet points
- Keep each bullet concise (1-2 lines)
- Maintain all important information
- Use parallel structure
- Optimize for mobile reading

Respond with the bulleted list only, no introductory text."""
        else:  # subheadings
            system_prompt = """You are a document formatting expert for government communications.
Add descriptive subheadings to the given text to make it skimmable.
- Create clear, descriptive subheadings for each major section
- Keep the original text but organize it under appropriate headings
- Use meaningful headings that help readers scan quickly
- Format as: ## Heading followed by the relevant text

Respond with the formatted text only."""
        
        user_prompt = f"Make this text skimmable using {format_type}: {text}"
        
        response = client.chat.completions.create(
            model=deployment_name,
            messages=[
                {"role": "system", "content": system_prompt},
                {"role": "user", "content": user_prompt}
            ],
            temperature=0.3,
            max_tokens=800
        )
        
        skimmable_text = response.choices[0].message.content.strip()
        
        return create_response(200, {
            'original': text,
            'skimmable': skimmable_text,
            'format': format_type,
            'ai_revised': True
        })
        
    except ValueError:
        return create_response(400, {'error': 'Invalid JSON'})
    except APITimeoutError:
        logging.error("Azure OpenAI timeout in make_skimmable function")
        return create_response(504, {'error': 'AI service timeout. Please try again.'})
    except RateLimitError:
        logging.error("Azure OpenAI rate limit exceeded in make_skimmable function")
        return create_response(429, {'error': 'Rate limit exceeded. Please try again later.'})
    except APIError as e:
        logging.error(f"Azure OpenAI API error in make_skimmable function: {type(e).__name__}")
        return create_response(502, {'error': 'AI service error. Please try again.'})
    except Exception as e:
        logging.error(f"Unexpected error in make_skimmable function: {type(e).__name__}")
        return create_response(500, {'error': 'Internal server error'})

@app.route(route="detect-jargon", methods=["POST"])
def detect_jargon(req: func.HttpRequest) -> func.HttpResponse:
    """
    Acronym & Jargon Detection
    
    Identifies county-specific jargon or undefined acronyms and suggests plain language alternatives.
    
    Request Body:
    {
        "text": "The text to analyze for jargon"
    }
    
    Response:
    {
        "original": "Original text",
        "jargon_found": [
            {
                "term": "PIO",
                "type": "acronym",
                "suggestion": "Public Information Officer",
                "plain_language": "communications staff member"
            }
        ],
        "ai_revised": false
    }
    """
    logging.info('Detect jargon function processed a request.')
    
    try:
        req_body = req.get_json()
        text = req_body.get('text')
        
        if not text:
            return create_response(400, {
                'error': 'Missing required field: text'
            })
        
        client = get_openai_client()
        deployment_name = os.environ.get("AZURE_OPENAI_DEPLOYMENT_NAME")
        
        system_prompt = """You are a Plain Language expert specializing in government communications.
Analyze the given text for:
1. Undefined acronyms
2. Government/technical jargon
3. Complex terminology that might confuse the general public

For each item found, provide:
- The term or acronym
- What type it is (acronym, jargon, technical_term)
- A definition if it's an acronym
- A plain language alternative

Format your response as a JSON array with this structure:
[
  {
    "term": "the jargon or acronym",
    "type": "acronym|jargon|technical_term",
    "definition": "what the acronym stands for (if applicable)",
    "plain_language": "simpler alternative"
  }
]

If no jargon is found, return an empty array: []
Respond ONLY with the JSON array, no other text."""
        
        user_prompt = f"Identify jargon and acronyms in this text: {text}"
        
        response = client.chat.completions.create(
            model=deployment_name,
            messages=[
                {"role": "system", "content": system_prompt},
                {"role": "user", "content": user_prompt}
            ],
            temperature=0.2,
            max_tokens=1000
        )
        
        jargon_response = response.choices[0].message.content.strip()
        
        # Parse the JSON response
        try:
            jargon_list = json.loads(jargon_response)
        except json.JSONDecodeError as e:
            # If parsing fails, log without exposing user content
            logging.warning(f"Failed to parse jargon detection response. Response length: {len(jargon_response)} chars")
            jargon_list = []
        
        return create_response(200, {
            'original': text,
            'jargon_found': jargon_list,
            'ai_revised': False
        })
        
    except ValueError:
        return create_response(400, {'error': 'Invalid JSON'})
    except APITimeoutError:
        logging.error("Azure OpenAI timeout in detect_jargon function")
        return create_response(504, {'error': 'AI service timeout. Please try again.'})
    except RateLimitError:
        logging.error("Azure OpenAI rate limit exceeded in detect_jargon function")
        return create_response(429, {'error': 'Rate limit exceeded. Please try again later.'})
    except APIError as e:
        logging.error(f"Azure OpenAI API error in detect_jargon function: {type(e).__name__}")
        return create_response(502, {'error': 'AI service error. Please try again.'})
    except Exception as e:
        logging.error(f"Unexpected error in detect_jargon function: {type(e).__name__}")
        return create_response(500, {'error': 'Internal server error'})

@app.route(route="health", methods=["GET"])
def health_check(req: func.HttpRequest) -> func.HttpResponse:
    """
    Health check endpoint for monitoring.
    
    Response:
    {
        "status": "healthy",
        "service": "londonstyle-ai-functions"
    }
    """
    logging.info('Health check processed.')
    
    return create_response(200, {
        'status': 'healthy',
        'service': 'londonstyle-ai-functions',
        'version': '1.0.0'
    })
