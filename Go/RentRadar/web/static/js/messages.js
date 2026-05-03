document.addEventListener('DOMContentLoaded', async () => {
    const uid = localStorage.getItem('userId');
    if (!uid) {
        window.location.href = '/login.html';
        return;
    }

    const esc =
        typeof escapeHtml === 'function'
            ? escapeHtml
            : function (s) {
                  if (!s) return '';
                  return String(s)
                      .replace(/&/g, '&amp;')
                      .replace(/</g, '&lt;')
                      .replace(/>/g, '&gt;')
                      .replace(/"/g, '&quot;')
                      .replace(/'/g, '&#39;');
              };

    function messagesHref(conversationId) {
        const path = window.location.pathname || '/messages.html';
        const q = new URLSearchParams(window.location.search);
        if (conversationId) q.set('conversation', String(conversationId));
        else q.delete('conversation');
        const s = q.toString();
        return s ? `${path}?${s}` : path;
    }

    function replaceMessagesHistory(conversationId) {
        const next = messagesHref(conversationId || undefined);
        if (window.location.pathname + window.location.search !== next) {
            history.replaceState(null, '', next);
        }
    }

    const convList = document.getElementById('convList');
    const chatHeader = document.getElementById('chatHeader');
    const chatMessages = document.getElementById('chatMessages');
    const chatForm = document.getElementById('chatForm');
    const chatInput = document.getElementById('chatInput');
    let currentConversationId = String(new URLSearchParams(window.location.search).get('conversation') || '').trim();

    function setChatEnabled(on) {
        chatInput.disabled = !on;
        const btn = chatForm.querySelector('button[type="submit"]');
        if (btn) btn.disabled = !on;
    }

    function showNoDialogsYet() {
        chatHeader.textContent = 'Нет диалогов';
        chatMessages.innerHTML =
            '<div style="padding:16px;"><p style="margin:0 0 10px;color:var(--text-color);">' +
            'У вас пока нет диалогов.</p><p style="margin:0;font-size:0.92rem;color:var(--text-muted);">' +
            'Откройте объявление на сайте и нажмите «Написать собственнику», чтобы начать переписку.</p></div>';
        setChatEnabled(false);
    }

    function clearConversationSelection() {
        currentConversationId = '';
        replaceMessagesHistory('');
        chatHeader.textContent = 'Выберите диалог';
        chatMessages.innerHTML =
            '<p style="color:var(--text-muted); padding:16px;">Выберите диалог в списке слева.</p>';
        setChatEnabled(false);
    }

    async function loadConversations() {
        const res = await fetch('/api/messages/conversations', { headers: { 'X-User-ID': uid } });
        const data = await res.json();
        if (!res.ok) throw new Error(data.error || 'Не удалось загрузить диалоги');
        convList.innerHTML = '';
        if (!Array.isArray(data) || data.length === 0) {
            convList.innerHTML =
                '<p style="padding:14px;line-height:1.45;color:var(--text-muted);">У вас пока нет диалогов.</p>';
            currentConversationId = '';
            replaceMessagesHistory('');
            showNoDialogsYet();
            return;
        }

        const idSet = new Set(data.map((c) => String(c.id)));

        data.forEach((c) => {
            const cid = String(c.id);
            const item = document.createElement('button');
            item.type = 'button';
            item.dataset.conversationId = cid;
            const selected = currentConversationId && cid === currentConversationId;
            item.style.cssText =
                'display:block;width:100%;text-align:left;padding:10px 12px;border:none;border-bottom:1px solid var(--border-color);cursor:pointer;' +
                (selected ? 'background:#eef8f6;' : 'background:white;');
            item.innerHTML = `<div style="font-weight:600;">${esc(c.listing_title || ('Объявление #' + c.listing_id))}</div><div style="font-size:0.85rem;color:var(--text-muted);">${esc(c.last_message || 'Без сообщений')}</div>`;
            item.addEventListener('click', () => {
                currentConversationId = cid;
                convList.querySelectorAll('button[data-conversation-id]').forEach((b) => {
                    b.style.background = b.dataset.conversationId === cid ? '#eef8f6' : 'white';
                });
                replaceMessagesHistory(cid);
                setChatEnabled(true);
                loadMessages();
            });
            convList.appendChild(item);
        });

        if (!currentConversationId || !idSet.has(currentConversationId)) {
            if (currentConversationId && !idSet.has(currentConversationId)) {
                replaceMessagesHistory('');
            }
            clearConversationSelection();
            return;
        }

        setChatEnabled(true);
    }

    async function loadMessages() {
        if (!currentConversationId) return;
        const res = await fetch(`/api/messages/${currentConversationId}`, { headers: { 'X-User-ID': uid } });
        const data = await res.json();
        if (!res.ok) throw new Error(data.error || 'Не удалось загрузить сообщения');
        chatHeader.textContent = `Диалог #${currentConversationId}`;
        chatMessages.innerHTML = '';
        if (!Array.isArray(data) || data.length === 0) {
            chatMessages.innerHTML = '<p style="color:var(--text-muted);">Сообщений пока нет</p>';
            return;
        }
        data.forEach((m) => {
            const mine = String(m.sender_id) === String(uid);
            const row = document.createElement('div');
            row.style.cssText = `margin-bottom:8px; display:flex; ${mine ? 'justify-content:flex-end;' : ''}`;
            row.innerHTML = `<div style="max-width:75%; background:${mine ? '#e8f3f2' : '#f3f4f6'}; padding:8px 10px; border-radius:10px;"><div style="font-size:0.8rem;color:var(--text-muted);">${esc(m.sender_name || '')}</div><div>${esc(m.body || '')}</div></div>`;
            chatMessages.appendChild(row);
        });
        chatMessages.scrollTop = chatMessages.scrollHeight;
    }

    chatForm.addEventListener('submit', async (e) => {
        e.preventDefault();
        if (!currentConversationId) return;
        const body = chatInput.value.trim();
        if (!body) return;
        const res = await fetch(`/api/messages/${currentConversationId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'X-User-ID': uid,
            },
            body: JSON.stringify({ body }),
        });
        const data = await res.json().catch(() => ({}));
        if (!res.ok) {
            alert(data.error || 'Не удалось отправить');
            return;
        }
        chatInput.value = '';
        await loadMessages();
        await loadConversations();
    });

    try {
        await loadConversations();
        if (currentConversationId) await loadMessages();
    } catch (e) {
        chatMessages.innerHTML = `<p class="error">${esc(e.message || 'Ошибка')}</p>`;
    }
});
