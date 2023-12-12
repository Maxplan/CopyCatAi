import React from "react";
import SyntaxHighlighter from "react-syntax-highlighter";
import { monokai } from 'react-syntax-highlighter/dist/esm/styles/hljs';

export const formatResponse = (text: string): JSX.Element[] => {
    const copyToClipboard = (textToCopy: string) => {
        navigator.clipboard.writeText(textToCopy).then(() => {
            console.log('Copied text to clipboard');
        }).catch(err => {
            console.error('Failed to copy text to clipboard', err);
        });
    };

    const processSegment = (segment: string, isCodeBlock: boolean, language: string) => {
        if (isCodeBlock) {
            formattedElements.push(
                <div key={`${segment}-code-block`} className="code-block">
                    <div className="code-topbar">
                        <p className="code-language">{language}</p>
                        <button onClick={() => copyToClipboard(segment.trim())} className="copy-button">copy</button>
                    </div>
                    <SyntaxHighlighter language={language} style={monokai}>
                        {segment.trim()}
                    </SyntaxHighlighter>
                </div>
            );
        } else {
            segment.split('\n').forEach((line, lineIndex) => {
                formattedElements.push(
                    <React.Fragment key={`${segment}-${lineIndex}`}>
                        {line}
                        {lineIndex < segment.split('\n').length - 1 && <br />}
                    </React.Fragment>
                );
            });
        }
    };

    // Array to hold JSX elements
    const formattedElements: JSX.Element[] = [];

    // Split the text into segments of code and non-code
    let lastIndex = 0;
    text.replace(/```(\w*)\n([\s\S]*?)```/g, (match, lang, code, index) => {
        // Add the non-code text before the current code block
        processSegment(text.substring(lastIndex, index), false, '');
        // Add the current code block
        processSegment(code, true, lang);
        lastIndex = index + match.length;
        return match; // This return is not used, but required by the replace function
    });

    // Add any remaining non-code text after the last code block
    if (lastIndex < text.length) {
        processSegment(text.substring(lastIndex), false, '');
    }

    return formattedElements;
};
