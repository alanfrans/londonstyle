# Front-End Web Page Requirements for London Style Writing Assistant

## 1. Executive Summary

This document defines the requirements for a front-end web application that interfaces with .NET 8 APIs to provide a Plain Language writing assistant for county employees. The application will analyze text for readability, highlight issues, and provide AI-powered suggestions for improvement.

---

## 2. User Interface Layout

### 2.1 Overall Layout Structure

The application shall use a responsive, modern single-page application (SPA) layout with the following main areas:

* **Header Bar** (Top) - Contains application branding, user information, and global controls
* **Metrics Dashboard** (Top or Side Panel) - Displays readability scores and statistics
* **Main Editor Area** (Center) - Primary text input and editing workspace
* **Sidebar/Action Panel** (Right or Collapsible) - Shows detailed suggestions and corrections
* **Footer** (Bottom) - Contains help links, privacy notices, and version information

### 2.2 Responsive Design Requirements

* **Desktop (≥1024px)**: Full three-column layout with visible metrics dashboard and action panel
* **Tablet (768px-1023px)**: Two-column layout with collapsible panels
* **Mobile (≤767px)**: Single-column layout with overlay panels for metrics and actions
* **Touch-Friendly**: All interactive elements must be at least 44×44 pixels for touch targets

---

## 3. Text Input Component

### 3.1 Main Text Editor

**Requirement:** The system shall provide a rich text input area that supports:

* **Minimum Height**: 400px on desktop, scalable on mobile
* **Character Limit**: At least 50,000 characters (approximately 10,000 words)
* **Real-Time Character Count**: Display current character/word count
* **Paste Support**: Accept plain text and formatted text from clipboard
* **Undo/Redo**: Standard undo/redo functionality (Ctrl+Z / Ctrl+Y)
* **Font**: Use clear, accessible font (e.g., system fonts or Inter, Open Sans)
* **Font Size**: Minimum 16px for body text
* **Line Height**: 1.5 to 1.7 for optimal readability

### 3.2 Text Import Options

The application shall support:

* **Paste from Clipboard**: Primary method for text entry
* **File Upload**: Support .txt, .docx files (optional enhancement)
* **Clear All**: Button to clear the entire text area with confirmation dialog

### 3.3 Auto-Save and Recovery

* **Auto-Save**: Automatically save text to browser localStorage every 30 seconds
* **Session Recovery**: Prompt user to restore previous session on page load if unsaved text exists
* **Privacy Mode**: Option to disable auto-save for sensitive content

---

## 4. Text Highlighting System

### 4.1 Highlighting Categories

The application shall visually highlight text issues using the following color-coded system:

| Issue Type | Color | Text Decoration | Description |
|------------|-------|-----------------|-------------|
| **Very Hard to Read** | Red background (rgba(255, 0, 0, 0.2)) | Bold | Sentences with Grade 16+ reading level |
| **Hard to Read** | Yellow background (rgba(255, 200, 0, 0.2)) | Underline | Sentences with Grade 10-15 reading level |
| **Simpler Alternative** | Purple/Blue background (rgba(147, 51, 234, 0.2)) | Dotted underline | Words with simpler alternatives available |
| **Grammar/Tense Issue** | Green background (rgba(34, 197, 94, 0.2)) | Wavy underline | Potential grammar, tense, or usage issues |
| **Passive Voice** | Orange background (rgba(249, 115, 22, 0.2)) | Double underline | Passive voice constructions |

### 4.2 Highlighting Behavior

* **Interactive Highlights**: Clicking/tapping a highlighted section should:
  - Show a tooltip or popup with explanation
  - Display suggested corrections
  - Provide "Fix" and "Ignore" buttons
* **Toggle Visibility**: Global toggle to show/hide all highlights
* **Category Filters**: Individual toggles for each highlight category
* **Hover Tooltips**: On desktop, hovering over a highlight shows quick information
* **Accessibility**: Highlights must not rely solely on color (use icons or text labels)

### 4.3 Highlight Legend

* **Requirement**: Display a collapsible legend explaining each highlight color and type
* **Location**: Top of editor area or in sidebar
* **Visual**: Small colored boxes with text labels and count of issues per category

---

## 5. Readability Metrics Dashboard

### 5.1 Header Panel Layout

The metrics dashboard shall be prominently displayed and include:

#### 5.1.1 Overall Grade Level Score
* **Primary Display**: Large, prominent display of overall grade level (e.g., "Grade 8")
* **Visual Indicator**: Color-coded badge:
  - Green: Grade 6-8 (Target range)
  - Yellow: Grade 9-10 (Acceptable)
  - Orange: Grade 11-12 (Needs improvement)
  - Red: Grade 13+ (Requires significant simplification)

#### 5.1.2 Detailed Readability Metrics

Display the following metrics in a card-based or grid layout:

| Metric | Description | Display Format |
|--------|-------------|----------------|
| **Flesch Reading Ease** | Score from 0-100 (higher = easier) | "72 (Fairly Easy)" |
| **Flesch-Kincaid Grade Level** | U.S. school grade level | "Grade 8.2" |
| **Gunning Fog Index** | Years of formal education needed | "10.4 years" |
| **SMOG Index** | Simple Measure of Gobbledygook | "Grade 9" |
| **Coleman-Liau Index** | Grade level based on characters | "Grade 7.8" |
| **Automated Readability Index** | Alternative grade level metric | "Grade 8.5" |

#### 5.1.3 Text Statistics

* **Word Count**: Total number of words
* **Sentence Count**: Total number of sentences
* **Paragraph Count**: Total number of paragraphs
* **Average Words per Sentence**: Word count / sentence count
* **Average Syllables per Word**: For complexity assessment
* **Reading Time**: Estimated time to read (assuming 200-250 words/minute)

#### 5.1.4 Issue Breakdown

Display counts for each issue category:

* Very Hard to Read Sentences: [X]
* Hard to Read Sentences: [Y]
* Complex Words: [Z]
* Grammar/Tense Issues: [W]
* Passive Voice Instances: [V]
* Total Issues: [Sum]

### 5.2 Target Audience Selector

* **Requirement**: Dropdown or button group to select target reading level:
  - General Public (Grade 6-8)
  - High School (Grade 9-10)
  - College (Grade 11-13)
  - Advanced (Grade 14+)
* **Action**: Adjusts highlighting thresholds and suggestions based on target
* **Default**: General Public (Grade 6-8)

### 5.3 Real-Time Updates

* **Requirement**: Metrics must update in real-time as user types or edits text
* **Debouncing**: Implement 500ms debounce to avoid excessive API calls
* **Loading State**: Show loading spinner or progress indicator during analysis

---

## 6. API Integration Points

### 6.1 Required .NET 8 API Endpoints

The front-end shall integrate with the following .NET 8 API endpoints:

#### 6.1.1 Text Analysis Endpoint
```
POST /api/analysis/analyze
Content-Type: application/json

Request Body:
{
  "text": "string",
  "targetGradeLevel": 8,
  "includeGrammarCheck": true,
  "includePassiveVoice": true
}

Response:
{
  "overallGrade": 10.2,
  "readabilityScores": {
    "fleschReadingEase": 65.5,
    "fleschKincaidGrade": 8.2,
    "gunningFog": 10.4,
    "smog": 9.0,
    "colemanLiau": 7.8,
    "automatedReadability": 8.5
  },
  "statistics": {
    "wordCount": 450,
    "sentenceCount": 23,
    "paragraphCount": 5,
    "avgWordsPerSentence": 19.6,
    "avgSyllablesPerWord": 1.8
  },
  "highlights": [
    {
      "id": "highlight-1",
      "type": "veryHardToRead",
      "startIndex": 50,
      "endIndex": 125,
      "text": "The aforementioned regulation...",
      "reason": "Grade 16+ reading level",
      "severity": "high"
    }
  ]
}
```

#### 6.1.2 Suggestion Endpoint
```
POST /api/suggestions/get-alternatives
Content-Type: application/json

Request Body:
{
  "highlightId": "highlight-1",
  "originalText": "string",
  "issueType": "veryHardToRead",
  "context": "surrounding text for context",
  "tone": "governmentProfessional"
}

Response:
{
  "suggestions": [
    {
      "id": "suggestion-1",
      "replacementText": "The rule mentioned earlier...",
      "explanation": "Simplified from Grade 16 to Grade 8",
      "newGradeLevel": 8.0,
      "confidence": 0.95
    },
    {
      "id": "suggestion-2",
      "replacementText": "This regulation...",
      "explanation": "Shortened and simplified",
      "newGradeLevel": 7.5,
      "confidence": 0.88
    }
  ],
  "aiGenerated": true,
  "timestamp": "2025-12-28T01:04:34Z"
}
```

#### 6.1.3 Apply Fix Endpoint
```
POST /api/suggestions/apply-fix
Content-Type: application/json

Request Body:
{
  "originalText": "string",
  "suggestionId": "suggestion-1",
  "highlightId": "highlight-1",
  "replacementText": "string",
  "startIndex": 50,
  "endIndex": 125
}

Response:
{
  "updatedText": "string",
  "success": true,
  "newAnalysis": { /* Updated analysis results */ }
}
```

#### 6.1.4 Batch Fix Endpoint
```
POST /api/suggestions/batch-fix
Content-Type: application/json

Request Body:
{
  "text": "string",
  "fixes": [
    {
      "highlightId": "highlight-1",
      "suggestionId": "suggestion-1"
    }
  ]
}

Response:
{
  "updatedText": "string",
  "appliedFixes": 5,
  "failedFixes": 0,
  "newAnalysis": { /* Updated analysis results */ }
}
```

#### 6.1.5 Word Alternatives Endpoint
```
GET /api/suggestions/word-alternatives?word={word}&context={context}

Response:
{
  "word": "utilize",
  "alternatives": [
    { "word": "use", "gradeLevel": 2, "frequency": "common" },
    { "word": "employ", "gradeLevel": 8, "frequency": "uncommon" }
  ]
}
```

### 6.2 API Communication Requirements

* **Protocol**: HTTPS only for production
* **Authentication**: Bearer token (JWT) via SSO integration
* **Error Handling**: Graceful degradation with user-friendly error messages
* **Timeout**: 10 second timeout with retry logic (max 2 retries)
* **Rate Limiting**: Respect API rate limits with user feedback
* **Caching**: Cache analysis results for identical text (5-minute TTL)

---

## 7. Interactive Fix Features

### 7.1 Individual Fix Actions

When user clicks/taps on a highlighted section:

**Action Panel Shall Display:**

1. **Issue Description**
   - Clear explanation of the problem
   - Current grade level or complexity score
   - Why it's flagged (e.g., "This sentence is 45 words long and has a Grade 16 reading level")

2. **Suggested Fixes**
   - Up to 3 AI-generated alternatives
   - Each suggestion shows:
     - Preview text
     - New grade level
     - "Apply" button
     - "Explain More" expandable section

3. **Manual Edit Option**
   - "Edit Manually" button that focuses the cursor on that text
   - Allows user to make their own changes

4. **Dismiss Options**
   - "Ignore This Issue" - Removes highlight for this instance
   - "Ignore All Similar" - Ignores this issue type globally (with toggle-back option)

### 7.2 Batch Fix Operations

**Requirement**: Provide options to fix multiple issues at once:

* **"Fix All [Category]" Buttons**:
  - "Simplify All Very Hard Sentences" - Applies best suggestion to all red highlights
  - "Replace All Complex Words" - Applies simpler alternatives to all purple highlights
  - "Fix All Grammar Issues" - Applies grammar corrections

* **Confirmation Dialog**: 
  - Show preview of changes before applying
  - List count of changes
  - Allow cancel operation

* **Undo Batch**: 
  - "Undo Last Batch" button to revert all changes from last batch operation

### 7.3 AI-Generated Content Labeling

**Requirement**: Per government transparency requirements:

* **Visual Indicator**: Any text modified by AI must show:
  - Yellow/gold border or background on the changed text
  - Small "AI" badge icon
  - Tooltip: "This text was revised by AI on [timestamp]"

* **Approval System**:
  - "Approve Change" button removes AI indicator
  - "Revert" button restores original text
  - Changes remain marked until explicitly approved by user

* **Change History**:
  - Track all AI suggestions applied
  - Provide "View History" panel showing before/after for each change
  - Export option for audit trail

---

## 8. Sidebar/Action Panel

### 8.1 Tabbed Interface

The sidebar shall include the following tabs:

#### Tab 1: Issues (Default)
* List of all current issues organized by category
* Click to jump to issue in text
* Show count per category
* Filter by category

#### Tab 2: Suggestions
* Active suggestion for currently selected highlight
* History of applied suggestions
* Pending suggestions

#### Tab 3: Settings
* Target reading level selector
* Tone selector (Government Professional / Community Friendly)
* Toggle highlight categories on/off
* Auto-save preferences
* Accessibility options

#### Tab 4: Help
* Guide on using the tool
* Explanation of readability metrics
* Examples of good vs. poor writing
* Contact support link

### 8.2 Collapsible Behavior

* **Desktop**: Always visible by default, can be collapsed to icon bar
* **Tablet/Mobile**: Overlay that slides in from right or bottom
* **Remember State**: Save collapsed/expanded state to localStorage

---

## 9. Accessibility Requirements (WCAG 2.1 AA Compliance)

### 9.1 Keyboard Navigation

* **Tab Order**: Logical tab order through all interactive elements
* **Keyboard Shortcuts**:
  - `Ctrl + Enter`: Analyze text
  - `Ctrl + Alt + N`: Next issue
  - `Ctrl + Alt + P`: Previous issue
  - `Esc`: Close popups/panels
  - `Ctrl + /`: Show keyboard shortcuts help

### 9.2 Screen Reader Support

* **ARIA Labels**: All interactive elements must have proper ARIA labels
* **Live Regions**: Announce analysis completion and changes
* **Alt Text**: All icons must have descriptive alt text
* **Landmark Regions**: Proper use of header, main, aside, footer elements

### 9.3 Color and Contrast

* **Contrast Ratios**: Minimum 4.5:1 for normal text, 3:1 for large text
* **Non-Color Indicators**: Don't rely solely on color for highlighting (use icons/patterns)
* **High Contrast Mode**: Support browser high contrast mode
* **Dark Mode**: Optional dark mode theme (nice-to-have)

### 9.4 Text Sizing

* **Zoom Support**: Support 200% zoom without loss of functionality
* **User Font Size**: Respect user's browser font size settings
* **No Fixed Sizes**: Use relative units (rem, em) not fixed pixels

---

## 10. Performance Requirements

### 10.1 Loading and Response Times

* **Initial Page Load**: < 2 seconds on 4G connection
* **Time to Interactive**: < 3 seconds
* **Analysis Response**: < 2 seconds for up to 5,000 words
* **UI Updates**: Highlighting updates within 100ms of API response

### 10.2 Optimization Strategies

* **Code Splitting**: Lazy load non-critical features
* **Asset Optimization**: Minify and compress CSS/JS
* **Image Optimization**: Use WebP format with fallbacks
* **Caching**: Cache static assets for 1 year with versioning
* **CDN**: Serve static assets from CDN (if available)

### 10.3 Progressive Enhancement

* **Core Functionality**: Basic text analysis works without JavaScript (graceful degradation)
* **Offline Support**: Optional PWA with Service Worker for basic offline functionality
* **Connection Handling**: Detect offline state and inform user

---

## 11. Technology Stack Recommendations

### 11.1 Front-End Framework Options

**Recommended Options** (in order of preference):

1. **Blazor WebAssembly**
   - Rationale: Native integration with .NET 8 backend, type safety, component reusability
   - Pros: C# throughout stack, strong typing, excellent tooling
   - Cons: Larger initial download size

2. **React with TypeScript**
   - Rationale: Most popular SPA framework, large ecosystem, excellent developer experience
   - Pros: Vast library ecosystem, great performance, huge community
   - Cons: Requires separate build tooling

3. **Vue.js 3 with TypeScript**
   - Rationale: Easier learning curve, excellent documentation, progressive framework
   - Pros: Simpler than React, great performance, flexible
   - Cons: Smaller ecosystem than React

4. **Angular 17+**
   - Rationale: Full-featured framework with TypeScript by default
   - Pros: Complete solution, strong structure, built-in features
   - Cons: Steeper learning curve, more opinionated

### 11.2 UI Component Library

* **Blazor**: Radzen Blazor Components, MudBlazor, or Blazorise
* **React**: Material-UI (MUI), Ant Design, or Chakra UI
* **Vue**: Vuetify, Element Plus, or Naive UI
* **Angular**: Angular Material or PrimeNG

### 11.3 Supporting Libraries

* **State Management**: 
  - React: Redux Toolkit or Zustand
  - Vue: Pinia
  - Blazor: Fluxor
  
* **HTTP Client**:
  - Axios (React/Vue)
  - HttpClient (Blazor)
  
* **Text Editor**:
  - Quill.js, Draft.js, or Slate.js for rich text
  - Or custom contentEditable div with highlight rendering
  
* **Animations**:
  - Framer Motion (React)
  - GSAP (framework agnostic)
  
* **Testing**:
  - Jest + React Testing Library (React)
  - Vitest + Vue Testing Library (Vue)
  - bUnit (Blazor)

---

## 12. User Workflows

### 12.1 Primary Workflow: Analyze and Fix Text

1. **User pastes text** into editor area
2. **System auto-analyzes** text after 500ms pause in typing
3. **Metrics dashboard updates** with readability scores
4. **Text highlights appear** showing issues
5. **User clicks a highlight** to see suggestions
6. **User selects a suggestion** or edits manually
7. **System applies fix** and re-analyzes affected text
8. **User approves AI changes** (removes AI indicator)
9. **User copies improved text** back to their document

### 12.2 Secondary Workflow: Batch Improvements

1. **User pastes large document**
2. **System analyzes** and shows high issue count
3. **User clicks "Fix All Very Hard Sentences"**
4. **System shows preview** of all proposed changes
5. **User confirms** batch operation
6. **System applies all fixes** with AI indicators
7. **User reviews changes** one by one
8. **User approves or reverts** individual changes
9. **Final text ready** for use

### 12.3 Tertiary Workflow: Manual Editing with Guidance

1. **User starts typing new content** in editor
2. **System provides real-time feedback** on grade level
3. **User sees live highlighting** as they type
4. **User adjusts writing** based on visual feedback
5. **System confirms** target grade level achieved
6. **User copies final text**

---

## 13. Security and Privacy Requirements

### 13.1 Client-Side Security

* **XSS Prevention**: Sanitize all user input before rendering
* **CSP Headers**: Implement Content Security Policy
* **HTTPS Only**: Force HTTPS in production
* **Secure Cookies**: Use secure, httpOnly cookies for sessions

### 13.2 Data Privacy

* **No Tracking**: No third-party analytics or tracking scripts
* **Local Storage Encryption**: Encrypt auto-saved text in localStorage
* **Clear Session**: Provide "Clear Session" button to remove all cached data
* **Privacy Notice**: Display notice about data handling per county policy

### 13.3 Authentication Integration

* **SSO Integration**: Authenticate via county SSO (SAML 2.0 or OAuth 2.0)
* **Session Timeout**: Auto-logout after 30 minutes of inactivity
* **Token Refresh**: Silently refresh authentication tokens
* **Unauthorized Access**: Redirect to login on auth failure

---

## 14. Browser Support

### 14.1 Minimum Supported Browsers

* **Chrome/Edge**: Last 2 versions (Chromium-based)
* **Firefox**: Last 2 versions
* **Safari**: Last 2 versions (macOS and iOS)

### 14.2 Polyfills and Fallbacks

* **ES6+ Features**: Include polyfills for older browsers if needed
* **CSS Grid/Flexbox**: Use with appropriate fallbacks
* **WebAssembly**: Fallback plan if Blazor WASM used

---

## 15. Deployment and DevOps

### 15.1 Build Requirements

* **Build Tool**: Webpack, Vite, or built-in framework CLI
* **Environment Variables**: Support for dev, staging, production configs
* **Source Maps**: Generate source maps for debugging
* **Minification**: Minify all production assets

### 15.2 Hosting Requirements

* **Static Hosting**: Application can be hosted as static files
* **API Proxy**: Support for API proxy configuration to avoid CORS
* **CDN Ready**: Assets can be served from CDN
* **Docker Support**: Optional Docker container for deployment

### 15.3 CI/CD Integration

* **Automated Builds**: Trigger builds on git push
* **Automated Tests**: Run unit and integration tests
* **Linting**: Enforce code quality standards
* **Deployment**: Automated deployment to staging/production

---

## 16. Testing Requirements

### 16.1 Unit Tests

* **Coverage Target**: Minimum 70% code coverage
* **Test Framework**: Jest, Vitest, or bUnit
* **Component Tests**: Test all React/Vue/Blazor components
* **Utility Functions**: Test all helper and utility functions

### 16.2 Integration Tests

* **API Integration**: Mock API responses and test integration
* **User Workflows**: Test complete user workflows
* **Error Scenarios**: Test error handling and edge cases

### 16.3 E2E Tests

* **Critical Paths**: Test main user workflows end-to-end
* **Framework**: Playwright or Cypress
* **Browsers**: Test in Chrome, Firefox, Safari

### 16.4 Accessibility Tests

* **Automated Scanning**: Use axe-core or Pa11y
* **Manual Testing**: Test with actual screen readers
* **Keyboard Navigation**: Verify all functionality accessible via keyboard

---

## 17. Documentation Requirements

### 17.1 User Documentation

* **User Guide**: Step-by-step guide with screenshots
* **Video Tutorial**: 3-5 minute overview video
* **FAQ**: Common questions and answers
* **Keyboard Shortcuts**: Printable reference card

### 17.2 Technical Documentation

* **Architecture Diagram**: Visual representation of system architecture
* **API Documentation**: Complete API reference (using Swagger/OpenAPI)
* **Component Documentation**: Document all reusable components
* **Setup Guide**: Developer setup instructions

---

## 18. Success Criteria

The front-end application will be considered successful when:

* ✅ Users can paste and analyze text within 2 seconds
* ✅ All highlight categories are clearly visible and distinguishable
* ✅ Readability metrics accurately reflect text complexity
* ✅ Users can apply individual and batch fixes successfully
* ✅ All AI-generated changes are clearly marked until approved
* ✅ Application meets WCAG 2.1 AA accessibility standards
* ✅ Application works on all required browsers and devices
* ✅ API integration is reliable with proper error handling
* ✅ User satisfaction score of 4+ out of 5 in usability testing

---

## 19. Future Enhancements (Out of Scope for MVP)

* **Document Export**: Export to PDF, Word, or other formats
* **Collaboration**: Real-time multi-user editing
* **Version History**: Track document revisions over time
* **Templates**: Pre-built templates for common document types
* **Browser Extension**: Chrome/Edge extension for analyzing any web page
* **Mobile Apps**: Native iOS/Android applications
* **Advanced AI Features**: Tone detection, bias detection, brand voice matching
* **Integrations**: Direct integration with Office 365, Google Docs
* **Advanced Analytics**: Department-wide readability analytics dashboard

---

## 20. Appendix: Wireframe Descriptions

### 20.1 Desktop Layout

```
┌────────────────────────────────────────────────────────────────┐
│ [Logo] London Style Assistant          [User] [Settings] [?]   │
├────────────────────────────────────────────────────────────────┤
│ ┌─────────────────────────────────┐  ┌────────────────────────┐ │
│ │ 📊 Readability Dashboard        │  │ 🔧 Actions             │ │
│ │                                  │  │                        │ │
│ │ Overall Grade: [Grade 10.2] 🟠  │  │ [📝 Issues: 15]        │ │
│ │                                  │  │ [💡 Suggestions]       │ │
│ │ 📈 Flesch Reading Ease: 65.5    │  │ [⚙️  Settings]         │ │
│ │ 📚 Words: 450  Sentences: 23    │  │ [❓ Help]              │ │
│ │                                  │  │                        │ │
│ │ Very Hard: 3  Hard: 8           │  │ [Simplify All]         │ │
│ │ Complex Words: 12               │  │ [Fix Grammar]          │ │
│ └─────────────────────────────────┘  └────────────────────────┘ │
│                                                                  │
│ ┌──────────────────────────────────────────────────────────────┐ │
│ │ 📝 Text Editor                                      [Clear]  │ │
│ │                                                              │ │
│ │  The aforementioned regulation stipulates that...           │ │
│ │  ━━━━━━━━━━━━━━━━━━━━━━━━                                   │ │
│ │  🔴 Very hard to read (Grade 16)                            │ │
│ │                                                              │ │
│ │  This sentence is much easier to understand.                │ │
│ │                                                              │ │
│ │  However, the utilization of complex terminology can        │ │
│ │           ━━━━━━━━━━━                                       │ │
│ │           🟣 Simpler: "use"                                 │ │
│ │                                                              │ │
│ │                                                              │ │
│ │                                                              │ │
│ │                                                              │ │
│ │                                                              │ │
│ └──────────────────────────────────────────────────────────────┘ │
│                                                                  │
│ [Legend: 🔴 Very Hard | 🟡 Hard | 🟣 Complex | 🟢 Grammar]        │
└────────────────────────────────────────────────────────────────┘
```

### 20.2 Mobile Layout

```
┌──────────────────────────┐
│ ☰ London Style      [👤] │
├──────────────────────────┤
│                          │
│ 📊 Grade: [10.2] 🟠      │
│ [View Details ▼]         │
│                          │
│ ┌────────────────────────┐ │
│ │ 📝 Text Editor         │ │
│ │                        │ │
│ │ The aforementioned...  │ │
│ │ ━━━━━━━━━━━━          │ │
│ │ 🔴 Very hard           │ │
│ │                        │ │
│ │ [Tap to see fix]       │ │
│ │                        │ │
│ │                        │ │
│ │                        │ │
│ └────────────────────────┘ │
│                          │
│ [📋 15 Issues] [🔧 Fix] │
│                          │
└──────────────────────────┘
```

---

## Document Version

* **Version**: 1.0
* **Date**: December 28, 2025
* **Author**: Development Team
* **Status**: Draft for Review
