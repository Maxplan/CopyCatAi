import React, { useState, useRef } from "react";
import "../../Styles/inputBar.css";
import uploadIcon from "../../imgs/add-document.svg";
import sendIcon from "../../imgs/send.svg";
import uploadIconAttached from "../../imgs/add-document-attached.svg";

interface InputBarProps {
    onSendMessage: (message: string, file?: File) => void;
}

const InputBar: React.FC<InputBarProps> = ({ onSendMessage }) => { 
    const [input, setInput] = useState("");
    const [file, setFile] = useState<File | null>(null);
    const textAreaRef = useRef<HTMLTextAreaElement>(null);

    const adjustTextAreaHeight = () => {
        if (textAreaRef.current) {
            textAreaRef.current.style.height = 'auto';
            const isOverflowing = textAreaRef.current.scrollHeight > textAreaRef.current.clientHeight;
            if (isOverflowing) {
                textAreaRef.current.style.height = textAreaRef.current.scrollHeight + 'px';
            } else {
                textAreaRef.current.style.height = '1.5em';
            }
        }
    };

    const handleInputChange = (e: React.ChangeEvent<HTMLTextAreaElement>) => {
        setInput(e.target.value);
        adjustTextAreaHeight();
    };

    const handleFileInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const selectedFile = e.target.files ? e.target.files[0] : null;
        setFile(selectedFile);
    };
    
    const handleSend = () => { 
        const trimmedInput = input.trim();
        if (trimmedInput || file) {
            onSendMessage(trimmedInput, file || undefined);
            setInput("");
            setFile(null);
            if (textAreaRef.current) {
                textAreaRef.current.style.height = '1.5em';
            }
        }
    };
    
    const handleKeyDown = (e: React.KeyboardEvent<HTMLTextAreaElement>) => {
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault();
            handleSend();
        }
    };

    return (
        <div className="inputBar">
            <input 
                type="file" 
                id="file-upload"
                onChange={handleFileInputChange}
                accept=".pdf" />
            {
                file 
                ? <label htmlFor="file-upload" className="custom-file-upload-attached">
                      <img src={uploadIconAttached} alt="Upload" />
                  </label>
                : <label htmlFor="file-upload" className="custom-file-upload">
                      <img src={uploadIcon} alt="Upload" />
                  </label>
            }
            <textarea 
                ref={textAreaRef}
                value={input} 
                onChange={handleInputChange}
                onKeyDown={handleKeyDown}
                placeholder="Message OpenAI"
            />
            <button onClick={handleSend} className="send-message-button">
                <img src={sendIcon} alt="send" />
            </button>
        </div>
    );
};

export default InputBar;
