#!/usr/bin/env python3
"""
Test script for London Style AI Function App
Run this locally to test all endpoints
"""

import requests
import json

# Configuration
BASE_URL = "http://localhost:7071/api"  # Change to your Azure URL for deployed testing

def print_response(title, response):
    """Pretty print API response"""
    print(f"\n{'='*60}")
    print(f"{title}")
    print('='*60)
    print(f"Status: {response.status_code}")
    try:
        print(json.dumps(response.json(), indent=2))
    except json.JSONDecodeError:
        print(response.text)

def test_health():
    """Test health check endpoint"""
    print("\n🏥 Testing Health Check...")
    response = requests.get(f"{BASE_URL}/health")
    print_response("Health Check", response)

def test_simplify():
    """Test simplify endpoint"""
    print("\n✍️  Testing Simplify...")
    data = {
        "text": "The implementation of the aforementioned policy was conducted by the department in accordance with established procedures."
    }
    response = requests.post(f"{BASE_URL}/simplify", json=data)
    print_response("Simplify", response)

def test_adjust_tone_professional():
    """Test tone adjustment (professional)"""
    print("\n🎭 Testing Tone Adjustment (Government Professional)...")
    data = {
        "text": "Hey folks, you gotta get a permit before you start building stuff.",
        "tone": "government_professional"
    }
    response = requests.post(f"{BASE_URL}/adjust-tone", json=data)
    print_response("Tone Adjustment (Professional)", response)

def test_adjust_tone_friendly():
    """Test tone adjustment (friendly)"""
    print("\n🎭 Testing Tone Adjustment (Community Friendly)...")
    data = {
        "text": "Residents must procure permits prior to the commencement of construction activities.",
        "tone": "community_friendly"
    }
    response = requests.post(f"{BASE_URL}/adjust-tone", json=data)
    print_response("Tone Adjustment (Friendly)", response)

def test_make_skimmable_bullets():
    """Test make skimmable (bullets)"""
    print("\n📋 Testing Make Skimmable (Bullets)...")
    data = {
        "text": "The county parks department offers various recreational programs including sports leagues, summer camps, and educational workshops. These programs are designed for all age groups and promote community engagement and healthy lifestyles. Registration is available online or in person at any community center. Payment plans are available for families who need financial assistance.",
        "format": "bullets"
    }
    response = requests.post(f"{BASE_URL}/make-skimmable", json=data)
    print_response("Make Skimmable (Bullets)", response)

def test_make_skimmable_subheadings():
    """Test make skimmable (subheadings)"""
    print("\n📋 Testing Make Skimmable (Subheadings)...")
    data = {
        "text": "The new traffic ordinance will affect downtown parking. All vehicles must be registered with the city parking authority. Overnight parking requires a special permit. Violations will result in citations and possible towing. Residents can apply for residential parking permits online. The permit fee is $50 per year.",
        "format": "subheadings"
    }
    response = requests.post(f"{BASE_URL}/make-skimmable", json=data)
    print_response("Make Skimmable (Subheadings)", response)

def test_detect_jargon():
    """Test jargon detection"""
    print("\n🔍 Testing Jargon Detection...")
    data = {
        "text": "The PIO will disseminate the RFP to all stakeholders for procurement of new infrastructure. The municipality will utilize best practices for the implementation."
    }
    response = requests.post(f"{BASE_URL}/detect-jargon", json=data)
    print_response("Jargon Detection", response)

def run_all_tests():
    """Run all tests"""
    print("\n" + "="*60)
    print("London Style AI Function App - Test Suite")
    print("="*60)
    
    try:
        test_health()
        test_simplify()
        test_adjust_tone_professional()
        test_adjust_tone_friendly()
        test_make_skimmable_bullets()
        test_make_skimmable_subheadings()
        test_detect_jargon()
        
        print("\n" + "="*60)
        print("✅ All tests completed!")
        print("="*60)
    except requests.exceptions.ConnectionError:
        print("\n❌ Error: Could not connect to the function app.")
        print("Make sure the function app is running:")
        print("  Local: func start")
        print(f"  URL: {BASE_URL}")
    except Exception as e:
        print(f"\n❌ Error: {str(e)}")

if __name__ == "__main__":
    run_all_tests()
