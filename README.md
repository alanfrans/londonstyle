"# London Style - Plain Language Writing Assistant

A comprehensive writing assistant tool designed for county employees to ensure public communications are clear, accessible, and compliant with Plain Language standards.

## Documentation

This repository contains requirements documentation for the London Style writing assistant system:

### 📄 [requirements.md](requirements.md)
Overall system requirements including:
- AI-powered rewriting and simplification features
- Tone and readability adjustment capabilities
- Data privacy and security requirements for government use
- Technical specifications and success metrics

### 🎨 [frontend-requirements.md](frontend-requirements.md)
Comprehensive front-end web application requirements including:
- **User Interface**: Responsive layout with text editor, metrics dashboard, and action panels
- **Text Highlighting**: Visual indicators for readability issues (very hard to read, hard to read, complex words, grammar issues)
- **Readability Metrics**: Grade level scoring, Flesch-Kincaid, Gunning Fog, and other readability indices
- **API Integration**: Detailed specifications for .NET 8 API endpoints
- **Interactive Features**: Individual and batch text correction capabilities
- **Accessibility**: WCAG 2.1 AA compliance requirements
- **Technology Stack**: Framework recommendations (Blazor, React, Vue, Angular)
- **User Workflows**: Step-by-step usage scenarios
- **Security & Privacy**: Government-compliant data handling

## Key Features

The front-end application will enable users to:

1. **Paste and Analyze Text** - Input text and receive instant readability analysis
2. **Visual Feedback** - See color-coded highlights for different issue types:
   - 🔴 Very Hard to Read (Grade 16+)
   - 🟡 Hard to Read (Grade 10-15)
   - 🟣 Complex Words (simpler alternatives available)
   - 🟢 Grammar/Tense Issues
   - 🟠 Passive Voice

3. **View Metrics** - Comprehensive readability dashboard showing:
   - Overall grade level
   - Multiple readability scores (Flesch-Kincaid, Gunning Fog, SMOG, etc.)
   - Word count, sentence count, and text statistics
   - Issue breakdown by category

4. **Apply Fixes** - Interactive correction options:
   - Click highlights to see AI-generated suggestions
   - Apply individual fixes
   - Batch fix operations for multiple issues
   - Manual editing with guidance

5. **AI Transparency** - Government-compliant change tracking:
   - Visual indicators for AI-modified text
   - Approval system for AI changes
   - Complete change history for audit trail

## Target Audience

- County employees and public information officers
- Government communicators
- Anyone creating public-facing communications requiring Plain Language compliance

## Technology

- **Backend**: .NET 8 APIs
- **Front-End**: Single Page Application (SPA) - framework TBD
- **Compliance**: WCAG 2.1 AA accessible, government data privacy standards
- **Target Grade Level**: 6th-8th grade reading level for general public communications" 
