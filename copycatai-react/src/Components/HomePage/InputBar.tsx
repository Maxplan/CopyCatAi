import React, { useState, useRef } from "react";
import "../../Styles/inputBar.css";

interface InputBarProps {
    onSendMessage: (message: string) => void;
}

const InputBar: React.FC<InputBarProps> = ({ onSendMessage }) => { 
    const [input, setInput] = useState("");
    const textAreaRef = useRef<HTMLTextAreaElement>(null);

    const adjustTextAreaHeight = () => {
        if (textAreaRef.current) {
            textAreaRef.current.style.height = 'auto'; // Reset height to recalculate
            const isOverflowing = textAreaRef.current.scrollHeight > textAreaRef.current.clientHeight;
            if (isOverflowing) {
                textAreaRef.current.style.height = textAreaRef.current.scrollHeight + 'px'; // Set new height
            } else {
                textAreaRef.current.style.height = '1.5em'; // Default height
            }
        }
    };

    const handleInputChange = (e: React.ChangeEvent<HTMLTextAreaElement>) => {
        setInput(e.target.value);
        adjustTextAreaHeight(); // Adjust textarea height dynamically
    };

    
    const handleSend = () => { 
        const trimmedInput = input.trim();

        if (trimmedInput) {
            onSendMessage(input);
            setInput("");
            if (textAreaRef.current) {
                textAreaRef.current.style.height = '1.5em'; // Reset height to default after sending message
            }  
        }
    };
    
    const handleKeyDown = (e: React.KeyboardEvent<HTMLTextAreaElement>) => {
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault(); // Prevent the default action (new line)
            handleSend();
        }
    };

    return (
        <div className="inputBar">
            <textarea 
                ref={textAreaRef}
                value={input} 
                onChange={handleInputChange}
                onKeyDown={handleKeyDown}
                placeholder="Message OpenAI"
            />
            <button onClick={handleSend}>Send</button>
        </div>
    );
};

export default InputBar;
